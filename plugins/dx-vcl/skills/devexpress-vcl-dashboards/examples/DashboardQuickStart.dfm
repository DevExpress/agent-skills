object DashboardQuickStartForm: TDashboardQuickStartForm
  Left = 0
  Top = 0
  Caption = 'VCL Dashboards Quick Start'
  ClientHeight = 500
  ClientWidth = 800
  Color = clBtnFace
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -12
  Font.Name = 'Segoe UI'
  Font.Style = []
  Position = poScreenCenter
  OnCreate = FormCreate
  TextHeight = 15
  object pnlTop: TPanel
    Left = 0
    Top = 0
    Width = 800
    Height = 44
    Align = alTop
    BevelOuter = bvNone
    TabOrder = 0
    object btnExport: TButton
      Left = 8
      Top = 8
      Width = 120
      Height = 28
      Caption = 'Export PDF'
      TabOrder = 0
      OnClick = btnExportClick
    end
  end
end
