#!/bin/bash 

#UNITY程序的路径#
UNITY_PATH=/Applications/Unity/Unity.app/Contents/MacOS/Unity

#游戏程序路径#
PROJECT_PATH=/Users/helpking/Desktop/01.WorkDir/99.Tools/UnityPlugins

# 导入外部指定宏
echo "$UNITY_PATH -projectPath $PROJECT_PATH -executeMethod CommandLine.DefinesSetting.AddAndroidDefines -projectName NFF -debug -defines LOCALIZATION_CN -batchmode -quit"
$UNITY_PATH -projectPath $PROJECT_PATH -executeMethod CommandLine.DefinesSetting.AddAndroidDefines -projectName NFF -debug -defines LOCALIZATION_CN -batchmode -quit

#在Unity中构建apk#
echo "$UNITY_PATH -projectPath $PROJECT_PATH -executeMethod CommandLine.ProjectBuild.BuildForAndroid -projectName UnityPlugins -gameName Sample -debug -batchmode -quit"
$UNITY_PATH -projectPath $PROJECT_PATH -executeMethod CommandLine.ProjectBuild.BuildForAndroid -projectName UnityPlugins -gameName Sample -debug -batchmode -quit

echo "Apk生成完毕"
