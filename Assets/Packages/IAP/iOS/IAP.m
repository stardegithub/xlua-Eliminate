//
//  IAP.m
//  Unity-iPhone
//
//  Created by Viktor Chernikov on 10/23/12.

#import <Foundation/Foundation.h>
#import <StoreKit/StoreKit.h>
#import <CommonCrypto/CommonDigest.h>
#import "VerificationController.h"

/**
 * @brief IAP阶段类型
 */
typedef NS_ENUM(NSUInteger, IAPErrorCode) {
    /**
     * @brief 无
     */
    None,
    /**
     * @brief 产品验证失败
     */
    ProductsValidationFailed,
    /**
     * @brief 购买失败
     */
    PurchaseFailed,
    /**
     * @brief 交易失败
     */
    TranscationFailed,

    /**
     * @brief 购买恢复失败
     */
    RestoreFailed
};

/**
 * @brief 购买状态
 */
typedef NS_ENUM(NSUInteger, TransactionState) {
    
    /**
     * @brief 购买中
     */
	TransactionStatePurchasing = 0,
    /**
     * @brief 购买完毕
     */
    TransactionStatePurchased,
    /**
     * @brief 交易取消
     */
    TransactionStateCanceled,
    /**
     * @brief 购买恢复
     */
    TransactionStateRestored,
    /**
     * @brief 购买延迟
     */
    TransactionStateDeferred,
    /**
     * @brief 未知
     */
    TransactionStateUnknown
};

/**
 * @brief 创建产品回调函数
 */
typedef void (*CreateProductCallback) (const char* title, const char* description, const char* identifier, float price, const char* priceString, const char* currency, const char* code, const char* locale, const char* countryCode);
/**
 * @brief 交易更新回调函数
 */
typedef void (*TransactionUpdatedCallback) (const char* productID, TransactionState state, const char* transactionID, const char* receipt, int32_t quantity);
/**
 * @brief 购买恢复完成回调函数
 */
typedef void (*RestoreFinishedCallback) ();
/**
 * @brief IAP失败回调函数
 */
typedef void (*IAPFailedCallback)(IAPErrorCode iIAPStage, int iErrorCode, const char* iErrorDetail);

static CreateProductCallback productsCallback = NULL;
static TransactionUpdatedCallback transactionCallback = NULL;
static RestoreFinishedCallback restoreCallback = NULL;
static IAPFailedCallback iapFailedCallback = NULL;


BOOL verifyPurchase (SKPaymentTransaction *transaction);

@interface IAPObserver : NSObject <SKProductsRequestDelegate, SKPaymentTransactionObserver> {
	BOOL requestingProducts;
}

- (void) requestProductsForIDs:(NSSet*)productIDs;

@property (nonatomic, retain) NSMutableSet *productIDs;
@property (nonatomic, retain) NSDictionary *validatedProducts;
@end

@implementation IAPObserver

- (void)dealloc
{
	[_productIDs release];
	[_validatedProducts release];
    [super dealloc];
}

/**
 * @brief 请求产品
 * @param productIDs 产品ID
 */
- (void)requestProductsForIDs:(NSSet*)productIDs
{
	if (!requestingProducts) {
		requestingProducts = YES;
        NSLog(@"NFF: (OC)requestProductsForIDs: Count: %ld", productIDs.count);
        
		SKProductsRequest *productsRequest = [[SKProductsRequest alloc] initWithProductIdentifiers:productIDs];
		productsRequest.delegate = self;
		[productsRequest start];
	}
}

/**
 * @brief 收到产品返回信息
 * @param request 请求
 * @param response 返回
 */
- (void)productsRequest:(SKProductsRequest *)request didReceiveResponse:(SKProductsResponse *)response
{
    
    NSLog(@"NFF: productsRequest0 Count:%ld", response.products.count);
	if (response.products) {
		[[NSNotificationCenter defaultCenter] postNotificationName:@"GluGotResponseForProductsRequest"
															object:nil
														  userInfo:@{ @"ResponseProducts": response.products }];
	}
	NSMutableDictionary *pairs = [NSMutableDictionary dictionary];
	NSNumberFormatter *currencyFormatter = [[NSNumberFormatter alloc] init];
	[currencyFormatter setFormatterBehavior:NSNumberFormatterBehavior10_4];
	[currencyFormatter setNumberStyle:NSNumberFormatterCurrencyStyle];
    NSLog(@"NFF: productsRequest1 Count:%ld", response.products.count);
    int idx = 0;
    unsigned long _count = response.products.count;
	for (SKProduct *product in response.products) {
		[pairs setObject:product forKey:product.productIdentifier];
		NSLocale* locale = product.priceLocale;
		[currencyFormatter setLocale:locale];
		(*productsCallback)([product.localizedTitle UTF8String],                              // -> 标题
                            [product.localizedDescription UTF8String],                        // -> 描述
                            [product.productIdentifier UTF8String],                           // -> ID
							[product.price floatValue],                                       // -> 价格
                            [[currencyFormatter stringFromNumber:product.price] UTF8String],  // -> 价格描述
							[[locale objectForKey:NSLocaleCurrencySymbol] UTF8String],        // -> 货币符号
							[[locale objectForKey:NSLocaleCurrencyCode] UTF8String],          // -> 货币代码
                            [locale.localeIdentifier UTF8String],                             // -> 本地ID
							[[locale objectForKey:NSLocaleCountryCode] UTF8String]);          // -> 国家代码
        
        NSLog(@"NFF: productsRequest1 (%d/%ld) %@", (idx + 1), _count, product.productIdentifier);
        ++idx;
	}
	[currencyFormatter release];
    // Only update validated products if new list is not empty
	if (pairs.count) {
		self.validatedProducts = pairs;
	}
	requestingProducts = NO;
	dispatch_async(dispatch_get_main_queue(), ^{
		(*productsCallback)(NULL, NULL, NULL, 0.0f, NULL, NULL, NULL, NULL, NULL);	          // 验证结束，特意加上一行空记录作为结束
	});
	[request release];
}

/**
 * @brief 请求失败
 * @param request 请求
 * @param error 错误
 */
- (void)request:(SKRequest *)request didFailWithError:(NSError *)error
{
	requestingProducts = NO;
	dispatch_async(dispatch_get_main_queue(), ^{
        // 验证失败，返回错误信息
        (*iapFailedCallback)(ProductsValidationFailed,                 // -> 产品验证失败
                             (int32_t)error.code,                        // -> ErrorCode
                             [error.localizedDescription UTF8String]);   // -> 错误描述
	});
	NSLog(@"NFF: products validation failed with error: %@", error);
	[request release];
}

#pragma mark - Transaction Observer

/**
 * @brief 监听交易购买结果
 * @param queue 购买队列
 * @param transactions 交易
 */
- (void)paymentQueue:(SKPaymentQueue *)queue updatedTransactions:(NSArray *)transactions
{
    for (SKPaymentTransaction *transaction in transactions) {
        switch (transaction.transactionState) {
            case SKPaymentTransactionStatePurchasing:
                dispatch_async(dispatch_get_main_queue(), ^{
                    (*transactionCallback) ([transaction.payment.productIdentifier UTF8String], // -> 产品ID
                                            TransactionStatePurchasing,                         // -> 交易状态：购买中
                                            [transaction.transactionIdentifier UTF8String],     // -> 交易ID
                                            NULL,                                               // -> 交易收据
                                            (int32_t)transaction.payment.quantity);             // -> 交易数量
                });
                break;
			case SKPaymentTransactionStatePurchased:
				dispatch_async(dispatch_get_main_queue(), ^{
					(*transactionCallback) ([transaction.payment.productIdentifier UTF8String], // -> 产品ID
                                            TransactionStatePurchased,                          // -> 交易状态：购买完毕
											[transaction.transactionIdentifier UTF8String],     // -> 交易ID
											[GluEncodeBase64(transaction.transactionReceipt.bytes, transaction.transactionReceipt.length) UTF8String], // -> 交易收据
                                            (int32_t)transaction.payment.quantity);             // -> 交易数量
				});
				break;
			case SKPaymentTransactionStateFailed:
				dispatch_async(dispatch_get_main_queue(), ^{
					NSLog(@"IAP: Transaction failed with error: %@", transaction.error);
                    // 取消
                    if(SKErrorPaymentCancelled == transaction.error.code) {
                        (*transactionCallback) ([transaction.payment.productIdentifier UTF8String], // -> 产品ID
                                                TransactionStateCanceled,                           // -> 交易状态：交易失败
                                                [transaction.transactionIdentifier UTF8String],     // -> 交易ID
                                                NULL,                                               // -> 交易收据
                                                (int32_t)transaction.payment.quantity);             // -> 交易数量
                    // 失败
                    } else {
                        (*iapFailedCallback) (TranscationFailed,
                                              (int32_t)transaction.error.code,
                                              [transaction.error.localizedDescription UTF8String]);
                    }
                    // 结束交易
					[[SKPaymentQueue defaultQueue] finishTransaction:transaction];
				});
				break;
			case SKPaymentTransactionStateRestored:
				dispatch_async(dispatch_get_main_queue(), ^{
					(*transactionCallback) ([transaction.payment.productIdentifier UTF8String], // -> 产品ID
                                            TransactionStateRestored,                           // -> 交易状态：购买恢复
											[transaction.transactionIdentifier UTF8String],     // -> 交易ID
											[GluEncodeBase64(transaction.transactionReceipt.bytes, transaction.transactionReceipt.length) UTF8String], // -> 交易收据
                                            (int32_t)transaction.payment.quantity);             // -> 交易数量
				});
				break;
#ifdef __IPHONE_8_0
			case SKPaymentTransactionStateDeferred:
				dispatch_async(dispatch_get_main_queue(), ^{
					(*transactionCallback) ([transaction.payment.productIdentifier UTF8String], // -> 产品ID
                                            TransactionStateDeferred,                           // -> 交易状态：未知购买状态
											[transaction.transactionIdentifier UTF8String],     // -> 交易ID
                                            NULL,                                               // -> 交易收据
                                            (int32_t)transaction.payment.quantity);             // -> 交易数量
				});
				break;
#endif
			default:
				NSLog(@"IAP: incorrect transaction state encountered");
				break;
        }
    }
}

- (void)paymentQueue:(SKPaymentQueue *)queue removedTransactions:(NSArray *)transactions
{
	NSLog(@"IAP: %lu transaction(s) removed from the queue", (unsigned long)transactions.count);
}

- (void)paymentQueue:(SKPaymentQueue *)queue restoreCompletedTransactionsFailedWithError:(NSError *)error
{
	dispatch_async(dispatch_get_main_queue(), ^{
		(*iapFailedCallback) (RestoreFailed, (int32_t)error.code, [error.localizedDescription UTF8String]);
	});
}

- (void)paymentQueueRestoreCompletedTransactionsFinished:(SKPaymentQueue *)queue
{
	dispatch_async(dispatch_get_main_queue(), ^{
		(*restoreCallback) ();
	});	
}

- (void)paymentQueue:(SKPaymentQueue *)queue updatedDownloads:(NSArray *)downloads
{
	NSLog(@"IAP: downloads updated");
}

- (void)resumed:(NSNotification *)notification
{
	[self requestProductsForIDs:self.productIDs];
}

@end

static IAPObserver *observer = nil;

#pragma mark - External interface
void IAPSetCallbacks(CreateProductCallback cb,
                     TransactionUpdatedCallback tcb,
                     RestoreFinishedCallback tfb,
                     IAPFailedCallback fb)
{
	productsCallback = cb;
	transactionCallback = tcb;
    restoreCallback = tfb;
    iapFailedCallback = fb;
	
	static dispatch_once_t onceToken;
	dispatch_once(&onceToken, ^{
		observer = [[IAPObserver alloc] init];
		[[NSNotificationCenter defaultCenter] addObserver:observer
												 selector:@selector(resumed:)
													 name:UIApplicationWillEnterForegroundNotification
												   object:nil];
	});
}

void IAPValidateProducts(const char* const * products, int16_t count)
{
	observer.productIDs = [NSMutableSet set];
    int _count = count;
    int idx = 0;
	if (products) {
		while (*products && count > 0) {
            
            NSLog(@"NFF: (OC)IAPValidateProducts: Add Product: (%d/%d) %@",
                  (idx+1), _count,
                  [[NSString alloc] initWithUTF8String:(*products)]);
            ++idx;
            
			[observer.productIDs addObject:@(*products)];
			++products;
			--count;
		}
	}
	[observer requestProductsForIDs:observer.productIDs];
}

void IAPBuyWithProductID(const char *productID, int32_t quantity)
{
	NSString *productIdentifier = @(productID);
	if ([observer.validatedProducts objectForKey:productIdentifier] != nil) {
		SKMutablePayment *payment = [SKMutablePayment paymentWithProduct:(SKProduct *)[observer.validatedProducts objectForKey:productIdentifier]];
		payment.quantity = quantity;
		[[SKPaymentQueue defaultQueue] addPayment:payment];
	} else {
		NSLog(@"IAP: attempt to purchase non-validated item %@, ignoring", productIdentifier);
	}
}

void IAPRestoreTransactions()
{
	[[SKPaymentQueue defaultQueue] restoreCompletedTransactions];
}

void IAPFinalizeTransaction(const char *transactionID)
{
	if (transactionID && *transactionID) {
		NSString* identifier = @(transactionID);
		SKPaymentQueue* queue = [SKPaymentQueue defaultQueue];
		for (SKPaymentTransaction *transaction in queue.transactions) {
			if ([transaction.transactionIdentifier isEqualToString:identifier]) {
				[queue finishTransaction:transaction];
				// Make a note of the fact that we've seen the transaction id already
				saveTransactionId(transaction.transactionIdentifier);
				return;
			}
		}
	}
}

/**
 * @brief 开始购买进程
 */
void IAPStartProcessing()
{
	SKPaymentQueue *queue = [SKPaymentQueue defaultQueue];
    // 添加购买监听
	[queue addTransactionObserver:observer];
	// Hack to tone down sandbox issues with stuck transactions
	[observer paymentQueue:queue updatedTransactions:queue.transactions];
}

/**
 * @brief 停止购买进程
 */
void IAPStopProcessing()
{
	[[SKPaymentQueue defaultQueue] removeTransactionObserver:observer];
}

/**
 * @brief 判断是否可以购买
 */
bool IAPIsEnabled()
{
	return [SKPaymentQueue canMakePayments];
}

#pragma mark - Purchase Verification

/**
 * @brief 验证交易
 * @param transactionID 交易ID
 */
bool IAPVerifyTransaction(const char *transactionID)
{
	if (transactionID && *transactionID) {
		NSString *identifier = @(transactionID);
		for (SKPaymentTransaction *transaction in [SKPaymentQueue defaultQueue].transactions) {
			if ([transaction.transactionIdentifier isEqualToString:identifier]) {
				if (transaction.transactionState == SKPaymentTransactionStatePurchased ||
					transaction.transactionState == SKPaymentTransactionStateRestored) {
					return GluVerifyPurchase(transaction);
				}
			}
		}
	}
	return false;
}

