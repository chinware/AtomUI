# AtomUI pt-BR 控件翻译审核

本文记录 AtomUI 首批巴西葡萄牙语公共控件翻译草稿。当前基线包含 4 个模块语言包、13 个 XLIFF 文件和
68 个非 obsolete unit。

## 审核状态

- 草稿生成日期：2026-08-07。
- 当前所有 target state 均为 `translated`，尚未获得巴西葡萄牙语人工审核。
- 官方模块项目仍设置 `AtomUILanguageMinimumState=final`，因此默认发布命令会阻止这些草稿进入正式包。
- 人工批准只允许把确认后的 target state 从 `translated` 改为 `final`；不得修改 Catalog `file id`、unit Key、
  英文 source、占位符或模块身份。
- `Calendar.YearSuffix` 的 source 为空；巴西葡萄牙语不需要年份后缀，因此 target 是经过明确标记的合法空字符串。

## 术语与参考

主要参考：

- `.referenceprojects/ant-design/components/locale/pt_BR.ts`
- `.referenceprojects/ant-design/components/calendar/locale/pt_BR.ts`
- `.referenceprojects/ant-design/components/date-picker/locale/pt_BR.ts`
- `.referenceprojects/ant-design/components/time-picker/locale/pt_BR.ts`

稳定术语：

| English | pt-BR |
|---|---|
| Cancel | Cancelar |
| Delete | Excluir |
| Loading | Carregando |
| No data | Sem dados |
| Select all | Selecionar tudo |
| Previous | Anterior |
| Next | Próximo |
| Finish | Concluir |
| Save | Salvar |
| Reset | Redefinir |

## AtomUI.Controls.I18n.PtBR

Catalog: `AtomUI.Controls.Localization.CommonLangResourceKind`

| Key | English source | Portuguese target | State |
|---|---|---|---|
| Cancel | Cancel | Cancelar | translated |
| Delete | Delete | Excluir | translated |
| Edit | Edit | Editar | translated |
| Loading | Loading | Carregando | translated |
| NoData | No data | Sem dados | translated |
| Ok | Ok | OK | translated |
| Optional | (optional) | (opcional) | translated |
| Reset | Reset | Redefinir | translated |
| Save | Save | Salvar | translated |
| Submit | Submit | Enviar | translated |

## AtomUI.Desktop.Controls.I18n.PtBR

### Calendar

Catalog: `AtomUI.Desktop.Controls.Localization.CalendarControlLangResourceKind`

| Key | English source | Portuguese target | State |
|---|---|---|---|
| Month | Month | Mês | translated |
| Week | Week | Semana | translated |
| Year | Year | Ano | translated |
| YearSuffix | `(empty string)` | `(empty string)` | translated |

### DatePicker

Catalog: `AtomUI.Desktop.Controls.Localization.DatePickerLangResourceKind`

| Key | English source | Portuguese target | State |
|---|---|---|---|
| Now | Now | Agora | translated |
| Today | Today | Hoje | translated |

### Dialog

Catalog: `AtomUI.Desktop.Controls.Localization.DialogLangResourceKind`

| Key | English source | Portuguese target | State |
|---|---|---|---|
| Abort | Abort | Interromper | translated |
| Apply | Apply | Aplicar | translated |
| Cancel | Cancel | Cancelar | translated |
| Close | Close | Fechar | translated |
| Discard | Discard | Descartar | translated |
| Help | Help | Ajuda | translated |
| Ignore | Ignore | Ignorar | translated |
| No | No | Não | translated |
| NoToAll | No to All | Não para todos | translated |
| Ok | OK | OK | translated |
| Open | Open | Abrir | translated |
| Reload | Reload | Recarregar | translated |
| Reset | Reset | Redefinir | translated |
| RestoreDefaults | Restore Defaults | Restaurar padrões | translated |
| Retry | Retry | Tentar novamente | translated |
| Save | Save | Salvar | translated |
| SaveAll | Save All | Salvar tudo | translated |
| Yes | Yes | Sim | translated |
| YesToAll | Yes to All | Sim para todos | translated |

### ImagePreviewer

Catalog: `AtomUI.Desktop.Controls.Localization.ImagePreviewerLangResourceKind`

| Key | English source | Portuguese target | State |
|---|---|---|---|
| ImageLoadFailed | Image load failed | Falha ao carregar a imagem | translated |
| Preview | Preview | Visualizar | translated |

### Pagination

Catalog: `AtomUI.Desktop.Controls.Localization.PaginationLangResourceKind`

| Key | English source | Portuguese target | State |
|---|---|---|---|
| JumpToText | Go to | Ir para | translated |
| PageText | Page | Página | translated |
| TotalInfoFormat | Total `${Total}` items | Total de `${Total}` itens | translated |

### QRCode

Catalog: `AtomUI.Desktop.Controls.Localization.QRCodeLangResourceKind`

| Key | English source | Portuguese target | State |
|---|---|---|---|
| Expired | QR code expired | Código QR expirado | translated |
| Refresh | Refresh | Atualizar | translated |
| Scanned | Scanned | Escaneado | translated |

### TimePicker

Catalog: `AtomUI.Desktop.Controls.Localization.TimePickerLangResourceKind`

| Key | English source | Portuguese target | State |
|---|---|---|---|
| AMText | AM | AM | translated |
| Now | Now | Agora | translated |
| PMText | PM | PM | translated |

### Tour

Catalog: `AtomUI.Desktop.Controls.Localization.TourLangResourceKind`

| Key | English source | Portuguese target | State |
|---|---|---|---|
| Finish | Finish | Concluir | translated |
| Next | Next | Próximo | translated |
| Previous | Previous | Anterior | translated |

### Transfer

Catalog: `AtomUI.Desktop.Controls.Localization.TransferLangResourceKind`

| Key | English source | Portuguese target | State |
|---|---|---|---|
| DeSelectAll | deselect all data | Desmarcar tudo | translated |
| InvertSelectCurrentPage | invert current page | Inverter seleção da página atual | translated |
| Item | item | item | translated |
| Items | items | itens | translated |
| RemoveAll | remove all data | Remover tudo | translated |
| RemoveCurrentPage | remove current page | Remover página atual | translated |
| SelectAll | select all data | Selecionar tudo | translated |
| SelectCurrentPage | select current page | Selecionar página atual | translated |

### Upload

Catalog: `AtomUI.Desktop.Controls.Localization.UploadLangResourceKind`

| Key | English source | Portuguese target | State |
|---|---|---|---|
| DragUploadHead | Click or drag file to this area to upload | Clique ou arraste o arquivo para esta área para enviar | translated |
| Pending | Pending... | Pendente... | translated |
| Uploading | Uploading... | Enviando... | translated |

## AtomUI.Desktop.Controls.DataGrid.I18n.PtBR

Catalog: `AtomUI.Desktop.Controls.DataGrid.Localization.DataGridLangResourceKind`

| Key | English source | Portuguese target | State |
|---|---|---|---|
| AscendTooltip | Click to sort ascending | Clique para ordenar em ordem crescente | translated |
| CancelConfirmText | Sure to cancel? | Tem certeza de que deseja cancelar? | translated |
| CancelTooltip | Click to cancel sorting | Clique para cancelar a ordenação | translated |
| DeleteConfirmText | Sure to delete? | Tem certeza de que deseja excluir? | translated |
| DescendTooltip | Click to sort descending | Clique para ordenar em ordem decrescente | translated |
| Operating | Operation in progress, please wait. | Operação em andamento, aguarde. | translated |
| SelectAllFilterItems | Select all items | Selecionar todos os itens | translated |

## AtomUI.Desktop.Controls.ColorPicker.I18n.PtBR

Catalog: `AtomUI.Desktop.Controls.ColorPicker.Localization.ColorPickerLangResourceKind`

| Key | English source | Portuguese target | State |
|---|---|---|---|
| EmptyColorText | Transparent | Transparente | translated |

## Aprovação

Revisor de pt-BR: `PENDING`

Data da aprovação: `PENDING`

Resultado: `PENDING`

Até que os três campos acima sejam preenchidos por um revisor humano, estes arquivos permanecem rascunhos `translated` e não
podem ser promovidos mecanicamente para `final`.
