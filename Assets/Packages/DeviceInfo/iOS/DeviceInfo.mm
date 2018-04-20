//
//  AutoResizeController.m
//  Unity-iPhone
//  屏幕自适应
//  Created by helpking on 2018/4/1.
//

#include <CoreGraphics/CoreGraphics.h>
#include "UnityAppController.h"
#include "UI/UnityView.h"

/**
 * @brief 宏定义：屏幕宽度
 */
#define SCREEN_WIDTH ([[UIScreen mainScreen] bounds].size.width)
/**
 * @brief 宏定义：屏幕高度
 */
#define SCREEN_HEIGHT ([[UIScreen mainScreen] bounds].size.height)
#define SCREEN_MAX_LENGTH (MAX(SCREEN_WIDTH, SCREEN_HEIGHT))
#define SCREEN_MIN_LENGTH (MIN(SCREEN_WIDTH, SCREEN_HEIGHT))

/**
 * @brief 宏定义：判断设备是否为iPad
 */
#define IS_IPAD (UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPad)
/**
 * @brief 宏定义：判断设备是否为iPhone
 */
#define IS_IPHONE (UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPhone)
/**
 * @brief 宏定义：判断设备是否为iPhone4或者更低型号设备
 */
#define IS_IPHONE_4_OR_LESS (IS_IPHONE && SCREEN_MAX_LENGTH < 568.0f)
/**
 * @brief 宏定义：判断设备是否为iPhone5
 */
#define IS_IPHONE_5 (IS_IPHONE && SCREEN_MAX_LENGTH == 568.0f)
/**
 * @brief 宏定义：判断设备是否为iPhone6
 */
#define IS_IPHONE_6 (IS_IPHONE && SCREEN_MAX_LENGTH == 667.0f)
/**
 * @brief 宏定义：判断设备是否为iPhone6P
 */
#define IS_IPHONE_6P (IS_IPHONE && SCREEN_MAX_LENGTH == 736.0f)

/**
 * @brief 宏定义：判断设备是否为Retina屏
 */
#define RETINA_SCALE ([[UIScreen mainScreen] scale])

/**
 * @brief 宏定义：判断设备是否为Retina屏
 */
#define IS_RETINA (RETINA_SCALE >= 2.0f)

/**
 * @brief 取得安全区域
 * @param[in] iView 视图
 * @return 安全区域
 */
CGRect CustomComputeSafeArea(UIView* iView) {
    CGSize screenSize = iView.bounds.size;
    CGRect screenRect = CGRectMake(0, 0, screenSize.width, screenSize.height);
    
    UIEdgeInsets insets = UIEdgeInsetsMake(0, 0, 0, 0);
    if ([iView respondsToSelector: @selector(safeAreaInsets)]) {
        if (@available(iOS 11.0, *)) {
            insets = [iView safeAreaInsets];
        } else {
            // Fallback on earlier versions
            insets = UIEdgeInsetsMake(0, 0, 0, 0);
        }
    }
    NSLog(@"[AutoResize] CustomComputeSafeArea: insets(Left:%f, Right:%f, Top:%f, Bottom:%f)",
          insets.left, insets.right, insets.top, insets.bottom);
    
    screenRect.origin.x += insets.left;
    screenRect.origin.y += insets.bottom; // Unity uses bottom left as the origin
    screenRect.size.width -= insets.left + insets.right;
    screenRect.size.height -= insets.top + insets.bottom;
    
    float scale = iView.contentScaleFactor;
    screenRect.origin.x *= scale;
    screenRect.origin.y *= scale;
    screenRect.size.width *= scale;
    screenRect.size.height *= scale;
    NSLog(@"[AutoResize] CustomComputeSafeArea: ScreenRect(X:%f, Y:%f, Width:%f, Height:%f)",
          screenRect.origin.x, screenRect.origin.y, screenRect.size.width, screenRect.size.height);
    return screenRect;
}

/**
 * @brief 取得安全区域
 * @param[in/out] iOriginX 原点:坐标X
 * @param[in/out] iOriginY 原点:坐标Y
 * @param[in/out] iWidth 宽度
 * @param[in/out] iHeight 高度
 */
extern "C" void _GetSafeAreaImpl(float* iOriginX, float* iOriginY, float* iWidth, float* iHeight) {
    UIView* view = GetAppController().unityView;
    CGRect area = CustomComputeSafeArea(view);
    *iOriginX = area.origin.x;
    *iOriginY = area.origin.y;
    *iWidth = area.size.width;
    *iHeight = area.size.height;
}

/**
 * @brief 取得边框余白（）
 * @param[in] iView 视图
 * @return 安全区域
 */
UIEdgeInsets _GetPaddingInfo(UIView* iView) {
    UIEdgeInsets _padding = UIEdgeInsetsMake(0, 0, 0, 0);
    if ([iView respondsToSelector: @selector(safeAreaInsets)]) {
        if (@available(iOS 11.0, *)) {
            _padding = [iView safeAreaInsets];
        } else {
            // Fallback on earlier versions
            _padding = UIEdgeInsetsMake(0, 0, 0, 0);
        }
    }
    NSLog(@"[AutoResize] _GetPaddingInfo: Padding(Left:%f, Right:%f, Top:%f, Bottom:%f)",
          _padding.left, _padding.right, _padding.top, _padding.bottom);
    return _padding;
}

/**
 * @brief 取得安全区域
 * @param[in/out] iOriginX 原点:坐标X
 * @param[in/out] iOriginY 原点:坐标Y
 * @param[in/out] iWidth 宽度
 * @param[in/out] iHeight 高度
 */
extern "C" void _GetSafePaddingImpl(float* ioLeft, float* ioRight, float* ioTop, float* ioBottom) {
    UIView* _view = GetAppController().unityView;
    
    // 一般屏幕为1.0f, Retina为2.0f或者3.0f
    float _scale = _view.contentScaleFactor;
    
    // 取得余白边距信息
    UIEdgeInsets _padding = _GetPaddingInfo(_view);
    *ioLeft = _padding.left;
    *ioRight = _padding.right;
    *ioTop = _padding.top;
    *ioBottom = _padding.bottom;
}

/**
 * @bbrief 判断是否为Retina屏幕
 * @return true:是; false:否;
 */
extern "C" bool _IsRetina() {
    return IS_RETINA;
}

/**
 * @bbrief 取得Retina屏幕的缩放率(一般为2.0f)
 * @return Retina屏幕的缩放率(一般为2.0f)
 */
extern "C" CGFloat _GetRetinaScale() {
    return RETINA_SCALE;
}

/**
 * @bbrief 取得屏幕宽度
 * @return 屏幕宽度
 */
extern "C" CGFloat _ScreenWidth() {
    return SCREEN_WIDTH;
}

/**
 * @bbrief 取得屏幕宽度
 * @return 屏幕宽度
 */
extern "C" CGFloat _ScreenHeight() {
    return SCREEN_HEIGHT;
}

/**
 * @bbrief 取得屏幕最大长度
 *         备注：屏幕最大长度 = MAX（屏幕宽度，屏幕高度）
 * @return 屏幕宽度最大长度
 */
extern "C" CGFloat _ScreenMaxLengh() {
    return SCREEN_MAX_LENGTH;
}

/**
 * @bbrief 取得屏幕最小长度
 *         备注：屏幕最小长度 = MIN（屏幕宽度，屏幕高度）
 * @return 屏幕宽度最小长度
 */
extern "C" CGFloat _ScreenMinLengh() {
    return SCREEN_MIN_LENGTH;
}

