# 概述

--------------------------------

## 目录

1.[打包信息设定文件](BuildInfo.md#1%E6%89%93%E5%8C%85%E4%BF%A1%E6%81%AF%E8%AE%BE%E5%AE%9A%E6%96%87%E4%BB%B6)

1.1.[打包信息项目说明](BuildInfo.md#11%E6%89%93%E5%8C%85%E4%BF%A1%E6%81%AF%E9%A1%B9%E7%9B%AE%E8%AF%B4%E6%98%8E)

2.[服务器设定](Servers.md#2%E6%9C%8D%E5%8A%A1%E5%99%A8%E8%AE%BE%E5%AE%9A)

2.1.[共通设定](Servers.md#21%E5%85%B1%E9%80%9A%E8%AE%BE%E5%AE%9A)

2.2.[上传服务器设定](Servers.md#22%E4%B8%8A%E4%BC%A0%E6%9C%8D%E5%8A%A1%E5%99%A8%E8%AE%BE%E5%AE%9A)

2.3.[下载服务器设定](Servers.md#23%E4%B8%8B%E8%BD%BD%E6%9C%8D%E5%8A%A1%E5%99%A8%E8%AE%BE%E5%AE%9A)

2.4.[进度条Tips](Servers.md#24%E8%BF%9B%E5%BA%A6%E6%9D%A1tips)

3.[资源打包](AssetBundle.md#3%E8%B5%84%E6%BA%90%E6%89%93%E5%8C%85)

3.1.[打包模式](AssetBundle.md#31%E6%89%93%E5%8C%85%E6%A8%A1%E5%BC%8F)

3.2.[设定文件](AssetBundle.md#32%E8%AE%BE%E5%AE%9A%E6%96%87%E4%BB%B6)

3.2.1.[项目设定说明](AssetBundle.md#321%E9%A1%B9%E7%9B%AE%E8%AE%BE%E5%AE%9A%E8%AF%B4%E6%98%8E)

3.2.2.[资源设定说明](AssetBundle.md#322%E8%B5%84%E6%BA%90%E8%AE%BE%E5%AE%9A%E8%AF%B4%E6%98%8E)

3.3.[打包步骤](AssetBundle.md#33%E6%89%93%E5%8C%85%E6%AD%A5%E9%AA%A4)

3.4.[打包依赖Map](AssetBundle.md#34%E6%89%93%E5%8C%85%E4%BE%9D%E8%B5%96map)

3.4.1.[项目设定说明](AssetBundle.md#341%E9%A1%B9%E7%9B%AE%E8%AE%BE%E5%AE%9A%E8%AF%B4%E6%98%8E)

3.4.2.[依赖Map设定说明](AssetBundle.md#342%E4%BE%9D%E8%B5%96map%E8%AE%BE%E5%AE%9A%E8%AF%B4%E6%98%8E)

4.[上传](Upload.md#4%E4%B8%8A%E4%BC%A0)

4.1.[上传脚本设定](Upload.md#41%E4%B8%8A%E4%BC%A0%E8%84%9A%E6%9C%AC%E8%AE%BE%E5%AE%9A)

4.2.[上传列表文件](Upload.md#42%E4%B8%8A%E4%BC%A0%E5%88%97%E8%A1%A8%E6%96%87%E4%BB%B6)

4.2.1.[上传列表项目说明](Upload.md#421%E4%B8%8A%E4%BC%A0%E5%88%97%E8%A1%A8%E9%A1%B9%E7%9B%AE%E8%AF%B4%E6%98%8E)

4.2.2.[上传目标项目说明](Upload.md#422%E4%B8%8A%E4%BC%A0%E7%9B%AE%E6%A0%87%E9%A1%B9%E7%9B%AE%E8%AF%B4%E6%98%8E)

4.2.3.[上传URL设定](Upload.md#423%E4%B8%8A%E4%BC%A0url%E8%AE%BE%E5%AE%9A)

4.2.3.1.[上传URL项目说明](Upload.md#4231%E4%B8%8A%E4%BC%A0url%E9%A1%B9%E7%9B%AE%E8%AF%B4%E6%98%8E)

5.[下载](Download.md#5%E4%B8%8B%E8%BD%BD)

5.1.[下载列表](Download.md#51%E4%B8%8B%E8%BD%BD%E5%88%97%E8%A1%A8)

5.1.1.[下载列表项目说明](Download.md#511%E4%B8%8B%E8%BD%BD%E5%88%97%E8%A1%A8%E9%A1%B9%E7%9B%AE%E8%AF%B4%E6%98%8E)

5.1.2.[下载目标项目说明](Download.md#512%E4%B8%8B%E8%BD%BD%E7%9B%AE%E6%A0%87%E9%A1%B9%E7%9B%AE%E8%AF%B4%E6%98%8E)

5.2.[下载URL设定](Download.md#52%E4%B8%8B%E8%BD%BDurl%E8%AE%BE%E5%AE%9A)

5.2.1.[下载URL项目说明](Download.md#521%E4%B8%8B%E8%BD%BDurl%E9%A1%B9%E7%9B%AE%E8%AF%B4%E6%98%8E)

6.[加载](LoadAssetBundles.md#6%E5%8A%A0%E8%BD%BD)

6.1.[加载接口](LoadAssetBundles.md#61%E5%8A%A0%E8%BD%BD%E6%8E%A5%E5%8F%A3)

6.1.1.[同步加载接口](LoadAssetBundles.md#611%E5%90%8C%E6%AD%A5%E5%8A%A0%E8%BD%BD%E6%8E%A5%E5%8F%A3)

6.1.2.[异步加载接口](LoadAssetBundles.md#612%E5%BC%82%E6%AD%A5%E5%8A%A0%E8%BD%BD%E6%8E%A5%E5%8F%A3)

7.[宏](Defines.md#7%E5%AE%8F)

8.[AndroidSDK](AndroidSDK.md#8androidsdk)

9.[XCode导出设定](XcodeSettings.md#9xcode%E5%AF%BC%E5%87%BA%E8%AE%BE%E5%AE%9A)

10.[组件](Component.md#10%E7%BB%84%E4%BB%B6)

99.[命令行](CommandLine.md#99%E5%91%BD%E4%BB%A4%E8%A1%8C)

99.1.[CI设定](CommandLine.md#991ci%E8%AE%BE%E5%AE%9A)

99.1.1.[TeamCity](CommandLine.md#9911teamcity)

99.2.[宏设定](CommandLine.md#992%E5%AE%8F%E8%AE%BE%E5%AE%9A)

99.3.[AssetBundle打包](CommandLine.md#993assetbundle%E6%89%93%E5%8C%85)

99.3.1.[手动打包AssetBundle](CommandLine.md#9931%E6%89%8B%E5%8A%A8%E6%89%93%E5%8C%85assetbundle)

99.3.2.[命令行打包AssetBundle](CommandLine.md#9932%E5%91%BD%E4%BB%A4%E8%A1%8C%E6%89%93%E5%8C%85assetbundle)

99.4.[App打包](CommandLine.md#994app%E6%89%93%E5%8C%85)

99.4.1.[App文件命名](CommandLine.md#9941app%E6%96%87%E4%BB%B6%E5%91%BD%E5%90%8D)

99.4.2.[iOS打包](CommandLine.md#9942ios%E6%89%93%E5%8C%85)

99.4.2.1.[手动打包iOS](CommandLine.md#99421%E6%89%8B%E5%8A%A8%E6%89%93%E5%8C%85ios)

99.4.2.2.[命令行打包iOS](CommandLine.md#99422%E5%91%BD%E4%BB%A4%E8%A1%8C%E6%89%93%E5%8C%85ios)

99.4.2.2.1.[导出Xcode工程](CommandLine.md#994221%E5%AF%BC%E5%87%BAxcode%E5%B7%A5%E7%A8%8B)

99.4.2.2.2.[导出ipa文件](CommandLine.md#994222%E5%AF%BC%E5%87%BAipa%E6%96%87%E4%BB%B6)

99.4.3.[Android打包](CommandLine.md#9943android%E6%89%93%E5%8C%85)

99.4.3.1.[手动打包Android](CommandLine.md#99431%E6%89%8B%E5%8A%A8%E6%89%93%E5%8C%85android)

99.4.3.2.[命令行打包Android](CommandLine.md#99432%E5%91%BD%E4%BB%A4%E8%A1%8C%E6%89%93%E5%8C%85android)

--------------------------------

