# MUI 定義區
;!define SetupCaption "$(SetupCaption)"

!define MUI_WELCOMEPAGE_TITLE "$(MUI_WELCOMEPAGE_TITLE)"
!define MUI_WELCOMEPAGE_TEXT "$(MUI_WELCOMEPAGE_TEXT)"

!define MUI_FINISHPAGE_TITLE "$(MUI_FINISHPAGE_TITLE)"
!define MUI_FINISHPAGE_TEXT "$(MUI_FINISHPAGE_TEXT)"

!define MUI_ABORTWARNING_TITLE "$(MUI_ABORTWARNING_TITLE)"
!define MUI_ABORTWARNING_TEXT "$(MUI_ABORTWARNING_TEXT)"

!define CHT 1028


# MUI 通用區
#=================================================================

LangString MUI_WELCOMEPAGE_TITLE ${CHT} "歡迎使用 $(^NameDA) 安裝精靈"
LangString MUI_WELCOMEPAGE_TEXT ${CHT} "這個精靈將會幫你完成 $(^NameDA) 的安裝。$\r$\n$\r$\n在開始安裝之前，建議先關閉其他所有應用程式。這將允許「安裝程式」更新指定的檔案，而不需要重新啟動你的電腦。$\r$\n$\r$\n請按下「下一步」繼續安裝。"

LangString MUI_FINISHPAGE_TITLE ${CHT} "安裝完成"
LangString MUI_FINISHPAGE_TEXT ${CHT} "安裝程式已成功地執行完成。"

LangString MUI_ABORTWARNING_TITLE ${CHT} "$(^Name) 安裝精靈"
LangString MUI_ABORTWARNING_TEXT ${CHT} "你確定要離開 $(^Name) 安裝程式？"

# 個別區
#=================================================================

LangString CAPTION_TEXT ${CHT} "$(^Name) 安裝精靈"
LangString INSTALLBUTTON_TEXT ${CHT} "確定(&O)"
