; 通用安裝安裝腳本
; 修改: ken670128
; 版本: 2.0
; 日期: 2013/05/19
; NSIS: 2.46
; 通用版


; 定義程式名稱和版本編號
;========================================================================================================
; ★ 程式的名稱 (若要改名稱的話改這裡)
!define PRODUCT_NAME "CrazyKTV"

; ★ 程式的版本 (改版時要改這裡)
!define PRODUCT_VERSION "v5.3.9"

; ★ 程式類型
!define PRODUCT_CAT "媒體工具\卡拉OK系統"

; ★ 程式執行檔及圖示
!define PRODUCT_EXE "CrazyKTV.exe"

; 設定壓縮格式
SetCompressor lzma

; 插入自訂語言
!include "Lang.nsi"

; 定義內部版
!define INSTALLER_STD

; 使用者權限
; RequestExecutionLevel none

; 多國語言支持
Unicode true

; 64位元系統支持
!include "x64.nsh"


; MUI 界面系統
;========================================================================================================
; 載入 MUI 系統
!include "MUI2.nsh"

; 設定 MUI 界面
!define MUI_ABORTWARNING
!define MUI_ICON "resource\CrazyKTV.ico"
!define MUI_UNICON "resource\uninst.ico"
!define MUI_HEADERIMAGE
!define MUI_HEADERIMAGE_BITMAP "resource\orange-header.bmp"
!define MUI_UNHEADERIMAGE_BITMAP "resource\orange-header-uninstall.bmp"
!define MUI_WELCOMEFINISHPAGE_BITMAP "resource\orange.bmp"
!define MUI_COMPONENTSPAGE_CHECKBITMAP "resource\plastic.bmp"

; 歡迎頁面
!insertmacro MUI_PAGE_WELCOME

; 安裝元件頁面
!insertmacro MUI_PAGE_COMPONENTS

; 安裝路徑頁面
!insertmacro MUI_PAGE_DIRECTORY

; 安裝檔案頁面
!insertmacro MUI_PAGE_INSTFILES

; 安裝結束頁面
!insertmacro MUI_PAGE_FINISH

; 解除安裝確認頁面
!insertmacro MUI_UNPAGE_CONFIRM

; 解除安裝檔案頁面
!insertmacro MUI_UNPAGE_INSTFILES

; 插入語言
!insertmacro MUI_LANGUAGE "TradChinese"


; 設定安裝項目
;========================================================================================================
; 設定安裝程式名稱
Name "${PRODUCT_NAME} ${PRODUCT_VERSION}"

; 設定安裝程式輸出檔案
!ifdef INSTALLER_STD
	OutFile ".\out\${PRODUCT_NAME}_Full.exe"
!endif

; 設定安裝類型
InstType "完整安裝"

; 設定安裝程式預設的安裝路徑
InstallDir "$EXEDIR\${PRODUCT_NAME}"

; 安裝主程式
	Section "${PRODUCT_NAME} 主程式" inst1
		SectionIn 1 RO
		SetShellVarContext all
		SetOutPath "$INSTDIR"
		File /r ".\include\*.*"
	SectionEnd

; 安裝到開始功能表
	!ifdef INSTALLER_STD
		Section "桌面及開始功能表群組" inst2
			SectionIn 1
			SetShellVarContext all
			SetOutPath "$INSTDIR"
			CreateDirectory "$SMPROGRAMS\${PRODUCT_NAME}"
			CreateShortCut "$SMPROGRAMS\${PRODUCT_NAME}\${PRODUCT_NAME}.lnk" "$INSTDIR\${PRODUCT_EXE}"
			CreateShortCut "$SMPROGRAMS\${PRODUCT_NAME}\解除安裝 ${PRODUCT_NAME}.lnk" "$INSTDIR\Uninstall.exe"
			CreateShortCut "$DESKTOP\${PRODUCT_NAME}.lnk" "$INSTDIR\${PRODUCT_EXE}"
		SectionEnd
	!endif



; 軟體個別設定
  Section "" soft
        ; 登錄
        ; WriteRegDWORD HKCU "" "" 0x00000000
  SectionEnd	

  
; 登錄設定
	Section "" end
		; 登錄解除安裝的資料
        WriteRegStr HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}" "DisplayName" "${PRODUCT_NAME}"
        WriteRegStr HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}" "DisplayIcon" "$INSTDIR\${PRODUCT_EXE}"
        WriteRegStr HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}" "UninstallString" "$\"$INSTDIR\Uninstall.exe$\""
        WriteRegStr HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}" "Installer Language" "$LANGUAGE"
        WriteUninstaller "$INSTDIR\Uninstall.exe"
	SectionEnd


; 設定安裝項目的描述
	!insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
		!insertmacro MUI_DESCRIPTION_TEXT ${inst1} "安裝 ${PRODUCT_NAME} 主程式到你的電腦裏。"
		!insertmacro MUI_DESCRIPTION_TEXT ${inst2} "加入開始功能表可以讓你更方便的執行 ${PRODUCT_NAME} 或解除安裝。"
	!insertmacro MUI_FUNCTION_DESCRIPTION_END


; 設定解除安裝項目
;========================================================================================================
	Section Uninstall
		; 要刪除的檔案和資料夾
		SetShellVarContext all
		!ifdef INSTALLER_STD
			RMDir /r "$SMPROGRAMS\${PRODUCT_NAME}"
		!endif
		
		Delete /REBOOTOK "$DESKTOP\${PRODUCT_NAME}.lnk"
		RMDir /r "$INSTDIR"
		
		; 刪除登錄機碼
		${If} ${RunningX64}
			DeleteRegKey HKCU "Software\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}"
		${Else}
			DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}"
		${EndIf}
	SectionEnd