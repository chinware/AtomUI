using System.Collections.Generic;
using AtomUI.Theme.Language;
using Avalonia.Controls;

namespace AtomUI.Theme.Language
{
    internal sealed class DatePickerEnUSLanguageProvider : LanguageProvider
    {
        public DatePickerEnUSLanguageProvider()
            : base(LanguageCode.en_US, "DatePicker")
        {
        }

        protected override System.Type GetResourceKindType()
        {
            return typeof(global::AtomUI.Desktop.Controls.Localization.DatePickerLangResourceKind);
        }

        public override void BuildResourceDictionary(IResourceDictionary dictionary)
        {
            var resourceKindType = GetResourceKindType();
            try
            {
                dictionary[global::AtomUI.Desktop.Controls.Localization.DatePickerLangResourceKind.Now] = global::AtomUI.Desktop.Controls.DatePickerLang.en_US.Now;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DatePickerLangResourceKind.Today] = global::AtomUI.Desktop.Controls.DatePickerLang.en_US.Today;
            }
            catch (System.Exception)
            {
                LogBuildResourceDictionaryError(resourceKindType);
                throw;
            }
        }
    }

    internal sealed class DatePickerZhCNLanguageProvider : LanguageProvider
    {
        public DatePickerZhCNLanguageProvider()
            : base(LanguageCode.zh_CN, "DatePicker")
        {
        }

        protected override System.Type GetResourceKindType()
        {
            return typeof(global::AtomUI.Desktop.Controls.Localization.DatePickerLangResourceKind);
        }

        public override void BuildResourceDictionary(IResourceDictionary dictionary)
        {
            var resourceKindType = GetResourceKindType();
            try
            {
                dictionary[global::AtomUI.Desktop.Controls.Localization.DatePickerLangResourceKind.Now] = global::AtomUI.Desktop.Controls.DatePickerLang.zh_CN.Now;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DatePickerLangResourceKind.Today] = global::AtomUI.Desktop.Controls.DatePickerLang.zh_CN.Today;
            }
            catch (System.Exception)
            {
                LogBuildResourceDictionaryError(resourceKindType);
                throw;
            }
        }
    }

    internal sealed class DatePickerZhTWLanguageProvider : LanguageProvider
    {
        public DatePickerZhTWLanguageProvider()
            : base(LanguageCode.zh_TW, "DatePicker")
        {
        }

        protected override System.Type GetResourceKindType()
        {
            return typeof(global::AtomUI.Desktop.Controls.Localization.DatePickerLangResourceKind);
        }

        public override void BuildResourceDictionary(IResourceDictionary dictionary)
        {
            var resourceKindType = GetResourceKindType();
            try
            {
                dictionary[global::AtomUI.Desktop.Controls.Localization.DatePickerLangResourceKind.Now] = global::AtomUI.Desktop.Controls.DatePickerLang.zh_TW.Now;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DatePickerLangResourceKind.Today] = global::AtomUI.Desktop.Controls.DatePickerLang.zh_TW.Today;
            }
            catch (System.Exception)
            {
                LogBuildResourceDictionaryError(resourceKindType);
                throw;
            }
        }
    }

    internal sealed class DialogEnUSLanguageProvider : LanguageProvider
    {
        public DialogEnUSLanguageProvider()
            : base(LanguageCode.en_US, "Dialog")
        {
        }

        protected override System.Type GetResourceKindType()
        {
            return typeof(global::AtomUI.Desktop.Controls.Localization.DialogLangResourceKind);
        }

        public override void BuildResourceDictionary(IResourceDictionary dictionary)
        {
            var resourceKindType = GetResourceKindType();
            try
            {
                dictionary[global::AtomUI.Desktop.Controls.Localization.DialogLangResourceKind.Abort] = global::AtomUI.Desktop.Controls.DialogLang.en_US.Abort;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DialogLangResourceKind.Apply] = global::AtomUI.Desktop.Controls.DialogLang.en_US.Apply;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DialogLangResourceKind.Cancel] = global::AtomUI.Desktop.Controls.DialogLang.en_US.Cancel;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DialogLangResourceKind.Close] = global::AtomUI.Desktop.Controls.DialogLang.en_US.Close;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DialogLangResourceKind.Discard] = global::AtomUI.Desktop.Controls.DialogLang.en_US.Discard;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DialogLangResourceKind.Help] = global::AtomUI.Desktop.Controls.DialogLang.en_US.Help;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DialogLangResourceKind.Ignore] = global::AtomUI.Desktop.Controls.DialogLang.en_US.Ignore;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DialogLangResourceKind.No] = global::AtomUI.Desktop.Controls.DialogLang.en_US.No;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DialogLangResourceKind.NoToAll] = global::AtomUI.Desktop.Controls.DialogLang.en_US.NoToAll;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DialogLangResourceKind.Ok] = global::AtomUI.Desktop.Controls.DialogLang.en_US.Ok;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DialogLangResourceKind.Open] = global::AtomUI.Desktop.Controls.DialogLang.en_US.Open;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DialogLangResourceKind.Reload] = global::AtomUI.Desktop.Controls.DialogLang.en_US.Reload;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DialogLangResourceKind.Reset] = global::AtomUI.Desktop.Controls.DialogLang.en_US.Reset;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DialogLangResourceKind.RestoreDefaults] = global::AtomUI.Desktop.Controls.DialogLang.en_US.RestoreDefaults;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DialogLangResourceKind.Retry] = global::AtomUI.Desktop.Controls.DialogLang.en_US.Retry;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DialogLangResourceKind.Save] = global::AtomUI.Desktop.Controls.DialogLang.en_US.Save;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DialogLangResourceKind.SaveAll] = global::AtomUI.Desktop.Controls.DialogLang.en_US.SaveAll;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DialogLangResourceKind.Yes] = global::AtomUI.Desktop.Controls.DialogLang.en_US.Yes;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DialogLangResourceKind.YesToAll] = global::AtomUI.Desktop.Controls.DialogLang.en_US.YesToAll;
            }
            catch (System.Exception)
            {
                LogBuildResourceDictionaryError(resourceKindType);
                throw;
            }
        }
    }

    internal sealed class DialogZhCNLanguageProvider : LanguageProvider
    {
        public DialogZhCNLanguageProvider()
            : base(LanguageCode.zh_CN, "Dialog")
        {
        }

        protected override System.Type GetResourceKindType()
        {
            return typeof(global::AtomUI.Desktop.Controls.Localization.DialogLangResourceKind);
        }

        public override void BuildResourceDictionary(IResourceDictionary dictionary)
        {
            var resourceKindType = GetResourceKindType();
            try
            {
                dictionary[global::AtomUI.Desktop.Controls.Localization.DialogLangResourceKind.Abort] = global::AtomUI.Desktop.Controls.DialogLang.zh_CN.Abort;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DialogLangResourceKind.Apply] = global::AtomUI.Desktop.Controls.DialogLang.zh_CN.Apply;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DialogLangResourceKind.Cancel] = global::AtomUI.Desktop.Controls.DialogLang.zh_CN.Cancel;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DialogLangResourceKind.Close] = global::AtomUI.Desktop.Controls.DialogLang.zh_CN.Close;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DialogLangResourceKind.Discard] = global::AtomUI.Desktop.Controls.DialogLang.zh_CN.Discard;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DialogLangResourceKind.Help] = global::AtomUI.Desktop.Controls.DialogLang.zh_CN.Help;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DialogLangResourceKind.Ignore] = global::AtomUI.Desktop.Controls.DialogLang.zh_CN.Ignore;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DialogLangResourceKind.No] = global::AtomUI.Desktop.Controls.DialogLang.zh_CN.No;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DialogLangResourceKind.NoToAll] = global::AtomUI.Desktop.Controls.DialogLang.zh_CN.NoToAll;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DialogLangResourceKind.Ok] = global::AtomUI.Desktop.Controls.DialogLang.zh_CN.Ok;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DialogLangResourceKind.Open] = global::AtomUI.Desktop.Controls.DialogLang.zh_CN.Open;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DialogLangResourceKind.Reload] = global::AtomUI.Desktop.Controls.DialogLang.zh_CN.Reload;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DialogLangResourceKind.Reset] = global::AtomUI.Desktop.Controls.DialogLang.zh_CN.Reset;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DialogLangResourceKind.RestoreDefaults] = global::AtomUI.Desktop.Controls.DialogLang.zh_CN.RestoreDefaults;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DialogLangResourceKind.Retry] = global::AtomUI.Desktop.Controls.DialogLang.zh_CN.Retry;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DialogLangResourceKind.Save] = global::AtomUI.Desktop.Controls.DialogLang.zh_CN.Save;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DialogLangResourceKind.SaveAll] = global::AtomUI.Desktop.Controls.DialogLang.zh_CN.SaveAll;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DialogLangResourceKind.Yes] = global::AtomUI.Desktop.Controls.DialogLang.zh_CN.Yes;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DialogLangResourceKind.YesToAll] = global::AtomUI.Desktop.Controls.DialogLang.zh_CN.YesToAll;
            }
            catch (System.Exception)
            {
                LogBuildResourceDictionaryError(resourceKindType);
                throw;
            }
        }
    }

    internal sealed class DialogZhTWLanguageProvider : LanguageProvider
    {
        public DialogZhTWLanguageProvider()
            : base(LanguageCode.zh_TW, "Dialog")
        {
        }

        protected override System.Type GetResourceKindType()
        {
            return typeof(global::AtomUI.Desktop.Controls.Localization.DialogLangResourceKind);
        }

        public override void BuildResourceDictionary(IResourceDictionary dictionary)
        {
            var resourceKindType = GetResourceKindType();
            try
            {
                dictionary[global::AtomUI.Desktop.Controls.Localization.DialogLangResourceKind.Abort] = global::AtomUI.Desktop.Controls.DialogLang.zh_TW.Abort;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DialogLangResourceKind.Apply] = global::AtomUI.Desktop.Controls.DialogLang.zh_TW.Apply;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DialogLangResourceKind.Cancel] = global::AtomUI.Desktop.Controls.DialogLang.zh_TW.Cancel;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DialogLangResourceKind.Close] = global::AtomUI.Desktop.Controls.DialogLang.zh_TW.Close;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DialogLangResourceKind.Discard] = global::AtomUI.Desktop.Controls.DialogLang.zh_TW.Discard;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DialogLangResourceKind.Help] = global::AtomUI.Desktop.Controls.DialogLang.zh_TW.Help;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DialogLangResourceKind.Ignore] = global::AtomUI.Desktop.Controls.DialogLang.zh_TW.Ignore;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DialogLangResourceKind.No] = global::AtomUI.Desktop.Controls.DialogLang.zh_TW.No;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DialogLangResourceKind.NoToAll] = global::AtomUI.Desktop.Controls.DialogLang.zh_TW.NoToAll;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DialogLangResourceKind.Ok] = global::AtomUI.Desktop.Controls.DialogLang.zh_TW.Ok;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DialogLangResourceKind.Open] = global::AtomUI.Desktop.Controls.DialogLang.zh_TW.Open;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DialogLangResourceKind.Reload] = global::AtomUI.Desktop.Controls.DialogLang.zh_TW.Reload;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DialogLangResourceKind.Reset] = global::AtomUI.Desktop.Controls.DialogLang.zh_TW.Reset;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DialogLangResourceKind.RestoreDefaults] = global::AtomUI.Desktop.Controls.DialogLang.zh_TW.RestoreDefaults;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DialogLangResourceKind.Retry] = global::AtomUI.Desktop.Controls.DialogLang.zh_TW.Retry;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DialogLangResourceKind.Save] = global::AtomUI.Desktop.Controls.DialogLang.zh_TW.Save;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DialogLangResourceKind.SaveAll] = global::AtomUI.Desktop.Controls.DialogLang.zh_TW.SaveAll;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DialogLangResourceKind.Yes] = global::AtomUI.Desktop.Controls.DialogLang.zh_TW.Yes;
                dictionary[global::AtomUI.Desktop.Controls.Localization.DialogLangResourceKind.YesToAll] = global::AtomUI.Desktop.Controls.DialogLang.zh_TW.YesToAll;
            }
            catch (System.Exception)
            {
                LogBuildResourceDictionaryError(resourceKindType);
                throw;
            }
        }
    }

    internal sealed class ImagePreviewerEnUSLanguageProvider : LanguageProvider
    {
        public ImagePreviewerEnUSLanguageProvider()
            : base(LanguageCode.en_US, "ImagePreviewer")
        {
        }

        protected override System.Type GetResourceKindType()
        {
            return typeof(global::AtomUI.Desktop.Controls.Localization.ImagePreviewerLangResourceKind);
        }

        public override void BuildResourceDictionary(IResourceDictionary dictionary)
        {
            var resourceKindType = GetResourceKindType();
            try
            {
                dictionary[global::AtomUI.Desktop.Controls.Localization.ImagePreviewerLangResourceKind.Preview] = global::AtomUI.Desktop.Controls.ImagePreviewerLang.en_US.Preview;
            }
            catch (System.Exception)
            {
                LogBuildResourceDictionaryError(resourceKindType);
                throw;
            }
        }
    }

    internal sealed class ImagePreviewerZhCNLanguageProvider : LanguageProvider
    {
        public ImagePreviewerZhCNLanguageProvider()
            : base(LanguageCode.zh_CN, "ImagePreviewer")
        {
        }

        protected override System.Type GetResourceKindType()
        {
            return typeof(global::AtomUI.Desktop.Controls.Localization.ImagePreviewerLangResourceKind);
        }

        public override void BuildResourceDictionary(IResourceDictionary dictionary)
        {
            var resourceKindType = GetResourceKindType();
            try
            {
                dictionary[global::AtomUI.Desktop.Controls.Localization.ImagePreviewerLangResourceKind.Preview] = global::AtomUI.Desktop.Controls.ImagePreviewerLang.zh_CN.Preview;
            }
            catch (System.Exception)
            {
                LogBuildResourceDictionaryError(resourceKindType);
                throw;
            }
        }
    }

    internal sealed class ImagePreviewerZhTWLanguageProvider : LanguageProvider
    {
        public ImagePreviewerZhTWLanguageProvider()
            : base(LanguageCode.zh_TW, "ImagePreviewer")
        {
        }

        protected override System.Type GetResourceKindType()
        {
            return typeof(global::AtomUI.Desktop.Controls.Localization.ImagePreviewerLangResourceKind);
        }

        public override void BuildResourceDictionary(IResourceDictionary dictionary)
        {
            var resourceKindType = GetResourceKindType();
            try
            {
                dictionary[global::AtomUI.Desktop.Controls.Localization.ImagePreviewerLangResourceKind.Preview] = global::AtomUI.Desktop.Controls.ImagePreviewerLang.zh_TW.Preview;
            }
            catch (System.Exception)
            {
                LogBuildResourceDictionaryError(resourceKindType);
                throw;
            }
        }
    }

    internal sealed class PaginationEnUSLanguageProvider : LanguageProvider
    {
        public PaginationEnUSLanguageProvider()
            : base(LanguageCode.en_US, "Pagination")
        {
        }

        protected override System.Type GetResourceKindType()
        {
            return typeof(global::AtomUI.Desktop.Controls.Localization.PaginationLangResourceKind);
        }

        public override void BuildResourceDictionary(IResourceDictionary dictionary)
        {
            var resourceKindType = GetResourceKindType();
            try
            {
                dictionary[global::AtomUI.Desktop.Controls.Localization.PaginationLangResourceKind.JumpToText] = global::AtomUI.Desktop.Controls.PaginationLang.en_US.JumpToText;
                dictionary[global::AtomUI.Desktop.Controls.Localization.PaginationLangResourceKind.PageText] = global::AtomUI.Desktop.Controls.PaginationLang.en_US.PageText;
                dictionary[global::AtomUI.Desktop.Controls.Localization.PaginationLangResourceKind.TotalInfoFormat] = global::AtomUI.Desktop.Controls.PaginationLang.en_US.TotalInfoFormat;
            }
            catch (System.Exception)
            {
                LogBuildResourceDictionaryError(resourceKindType);
                throw;
            }
        }
    }

    internal sealed class PaginationZhCNLanguageProvider : LanguageProvider
    {
        public PaginationZhCNLanguageProvider()
            : base(LanguageCode.zh_CN, "Pagination")
        {
        }

        protected override System.Type GetResourceKindType()
        {
            return typeof(global::AtomUI.Desktop.Controls.Localization.PaginationLangResourceKind);
        }

        public override void BuildResourceDictionary(IResourceDictionary dictionary)
        {
            var resourceKindType = GetResourceKindType();
            try
            {
                dictionary[global::AtomUI.Desktop.Controls.Localization.PaginationLangResourceKind.JumpToText] = global::AtomUI.Desktop.Controls.PaginationLang.zh_CN.JumpToText;
                dictionary[global::AtomUI.Desktop.Controls.Localization.PaginationLangResourceKind.PageText] = global::AtomUI.Desktop.Controls.PaginationLang.zh_CN.PageText;
                dictionary[global::AtomUI.Desktop.Controls.Localization.PaginationLangResourceKind.TotalInfoFormat] = global::AtomUI.Desktop.Controls.PaginationLang.zh_CN.TotalInfoFormat;
            }
            catch (System.Exception)
            {
                LogBuildResourceDictionaryError(resourceKindType);
                throw;
            }
        }
    }

    internal sealed class PaginationZhTWLanguageProvider : LanguageProvider
    {
        public PaginationZhTWLanguageProvider()
            : base(LanguageCode.zh_TW, "Pagination")
        {
        }

        protected override System.Type GetResourceKindType()
        {
            return typeof(global::AtomUI.Desktop.Controls.Localization.PaginationLangResourceKind);
        }

        public override void BuildResourceDictionary(IResourceDictionary dictionary)
        {
            var resourceKindType = GetResourceKindType();
            try
            {
                dictionary[global::AtomUI.Desktop.Controls.Localization.PaginationLangResourceKind.JumpToText] = global::AtomUI.Desktop.Controls.PaginationLang.zh_TW.JumpToText;
                dictionary[global::AtomUI.Desktop.Controls.Localization.PaginationLangResourceKind.PageText] = global::AtomUI.Desktop.Controls.PaginationLang.zh_TW.PageText;
                dictionary[global::AtomUI.Desktop.Controls.Localization.PaginationLangResourceKind.TotalInfoFormat] = global::AtomUI.Desktop.Controls.PaginationLang.zh_TW.TotalInfoFormat;
            }
            catch (System.Exception)
            {
                LogBuildResourceDictionaryError(resourceKindType);
                throw;
            }
        }
    }

    internal sealed class QRCodeEnUSLanguageProvider : LanguageProvider
    {
        public QRCodeEnUSLanguageProvider()
            : base(LanguageCode.en_US, "QRCode")
        {
        }

        protected override System.Type GetResourceKindType()
        {
            return typeof(global::AtomUI.Desktop.Controls.Localization.QRCodeLangResourceKind);
        }

        public override void BuildResourceDictionary(IResourceDictionary dictionary)
        {
            var resourceKindType = GetResourceKindType();
            try
            {
                dictionary[global::AtomUI.Desktop.Controls.Localization.QRCodeLangResourceKind.Expired] = global::AtomUI.Desktop.Controls.QRCodeLang.en_US.Expired;
                dictionary[global::AtomUI.Desktop.Controls.Localization.QRCodeLangResourceKind.Refresh] = global::AtomUI.Desktop.Controls.QRCodeLang.en_US.Refresh;
                dictionary[global::AtomUI.Desktop.Controls.Localization.QRCodeLangResourceKind.Scanned] = global::AtomUI.Desktop.Controls.QRCodeLang.en_US.Scanned;
            }
            catch (System.Exception)
            {
                LogBuildResourceDictionaryError(resourceKindType);
                throw;
            }
        }
    }

    internal sealed class QRCodeZhCNLanguageProvider : LanguageProvider
    {
        public QRCodeZhCNLanguageProvider()
            : base(LanguageCode.zh_CN, "QRCode")
        {
        }

        protected override System.Type GetResourceKindType()
        {
            return typeof(global::AtomUI.Desktop.Controls.Localization.QRCodeLangResourceKind);
        }

        public override void BuildResourceDictionary(IResourceDictionary dictionary)
        {
            var resourceKindType = GetResourceKindType();
            try
            {
                dictionary[global::AtomUI.Desktop.Controls.Localization.QRCodeLangResourceKind.Expired] = global::AtomUI.Desktop.Controls.QRCodeLang.zh_CN.Expired;
                dictionary[global::AtomUI.Desktop.Controls.Localization.QRCodeLangResourceKind.Refresh] = global::AtomUI.Desktop.Controls.QRCodeLang.zh_CN.Refresh;
                dictionary[global::AtomUI.Desktop.Controls.Localization.QRCodeLangResourceKind.Scanned] = global::AtomUI.Desktop.Controls.QRCodeLang.zh_CN.Scanned;
            }
            catch (System.Exception)
            {
                LogBuildResourceDictionaryError(resourceKindType);
                throw;
            }
        }
    }

    internal sealed class QRCodeZhTWLanguageProvider : LanguageProvider
    {
        public QRCodeZhTWLanguageProvider()
            : base(LanguageCode.zh_TW, "QRCode")
        {
        }

        protected override System.Type GetResourceKindType()
        {
            return typeof(global::AtomUI.Desktop.Controls.Localization.QRCodeLangResourceKind);
        }

        public override void BuildResourceDictionary(IResourceDictionary dictionary)
        {
            var resourceKindType = GetResourceKindType();
            try
            {
                dictionary[global::AtomUI.Desktop.Controls.Localization.QRCodeLangResourceKind.Expired] = global::AtomUI.Desktop.Controls.QRCodeLang.zh_TW.Expired;
                dictionary[global::AtomUI.Desktop.Controls.Localization.QRCodeLangResourceKind.Refresh] = global::AtomUI.Desktop.Controls.QRCodeLang.zh_TW.Refresh;
                dictionary[global::AtomUI.Desktop.Controls.Localization.QRCodeLangResourceKind.Scanned] = global::AtomUI.Desktop.Controls.QRCodeLang.zh_TW.Scanned;
            }
            catch (System.Exception)
            {
                LogBuildResourceDictionaryError(resourceKindType);
                throw;
            }
        }
    }

    internal sealed class TimePickerEnUSLanguageProvider : LanguageProvider
    {
        public TimePickerEnUSLanguageProvider()
            : base(LanguageCode.en_US, "TimePicker")
        {
        }

        protected override System.Type GetResourceKindType()
        {
            return typeof(global::AtomUI.Desktop.Controls.Localization.TimePickerLangResourceKind);
        }

        public override void BuildResourceDictionary(IResourceDictionary dictionary)
        {
            var resourceKindType = GetResourceKindType();
            try
            {
                dictionary[global::AtomUI.Desktop.Controls.Localization.TimePickerLangResourceKind.AMText] = global::AtomUI.Desktop.Controls.TimePickerLang.en_US.AMText;
                dictionary[global::AtomUI.Desktop.Controls.Localization.TimePickerLangResourceKind.Now] = global::AtomUI.Desktop.Controls.TimePickerLang.en_US.Now;
                dictionary[global::AtomUI.Desktop.Controls.Localization.TimePickerLangResourceKind.PMText] = global::AtomUI.Desktop.Controls.TimePickerLang.en_US.PMText;
            }
            catch (System.Exception)
            {
                LogBuildResourceDictionaryError(resourceKindType);
                throw;
            }
        }
    }

    internal sealed class TimePickerZhCNLanguageProvider : LanguageProvider
    {
        public TimePickerZhCNLanguageProvider()
            : base(LanguageCode.zh_CN, "TimePicker")
        {
        }

        protected override System.Type GetResourceKindType()
        {
            return typeof(global::AtomUI.Desktop.Controls.Localization.TimePickerLangResourceKind);
        }

        public override void BuildResourceDictionary(IResourceDictionary dictionary)
        {
            var resourceKindType = GetResourceKindType();
            try
            {
                dictionary[global::AtomUI.Desktop.Controls.Localization.TimePickerLangResourceKind.AMText] = global::AtomUI.Desktop.Controls.TimePickerLang.zh_CN.AMText;
                dictionary[global::AtomUI.Desktop.Controls.Localization.TimePickerLangResourceKind.Now] = global::AtomUI.Desktop.Controls.TimePickerLang.zh_CN.Now;
                dictionary[global::AtomUI.Desktop.Controls.Localization.TimePickerLangResourceKind.PMText] = global::AtomUI.Desktop.Controls.TimePickerLang.zh_CN.PMText;
            }
            catch (System.Exception)
            {
                LogBuildResourceDictionaryError(resourceKindType);
                throw;
            }
        }
    }

    internal sealed class TimePickerZhTWLanguageProvider : LanguageProvider
    {
        public TimePickerZhTWLanguageProvider()
            : base(LanguageCode.zh_TW, "TimePicker")
        {
        }

        protected override System.Type GetResourceKindType()
        {
            return typeof(global::AtomUI.Desktop.Controls.Localization.TimePickerLangResourceKind);
        }

        public override void BuildResourceDictionary(IResourceDictionary dictionary)
        {
            var resourceKindType = GetResourceKindType();
            try
            {
                dictionary[global::AtomUI.Desktop.Controls.Localization.TimePickerLangResourceKind.AMText] = global::AtomUI.Desktop.Controls.TimePickerLang.zh_TW.AMText;
                dictionary[global::AtomUI.Desktop.Controls.Localization.TimePickerLangResourceKind.Now] = global::AtomUI.Desktop.Controls.TimePickerLang.zh_TW.Now;
                dictionary[global::AtomUI.Desktop.Controls.Localization.TimePickerLangResourceKind.PMText] = global::AtomUI.Desktop.Controls.TimePickerLang.zh_TW.PMText;
            }
            catch (System.Exception)
            {
                LogBuildResourceDictionaryError(resourceKindType);
                throw;
            }
        }
    }

    internal sealed class TourEnUSLanguageProvider : LanguageProvider
    {
        public TourEnUSLanguageProvider()
            : base(LanguageCode.en_US, "Tour")
        {
        }

        protected override System.Type GetResourceKindType()
        {
            return typeof(global::AtomUI.Desktop.Controls.Localization.TourLangResourceKind);
        }

        public override void BuildResourceDictionary(IResourceDictionary dictionary)
        {
            var resourceKindType = GetResourceKindType();
            try
            {
                dictionary[global::AtomUI.Desktop.Controls.Localization.TourLangResourceKind.Finish] = global::AtomUI.Desktop.Controls.TourLang.en_US.Finish;
                dictionary[global::AtomUI.Desktop.Controls.Localization.TourLangResourceKind.Next] = global::AtomUI.Desktop.Controls.TourLang.en_US.Next;
                dictionary[global::AtomUI.Desktop.Controls.Localization.TourLangResourceKind.Previous] = global::AtomUI.Desktop.Controls.TourLang.en_US.Previous;
            }
            catch (System.Exception)
            {
                LogBuildResourceDictionaryError(resourceKindType);
                throw;
            }
        }
    }

    internal sealed class TourZhCNLanguageProvider : LanguageProvider
    {
        public TourZhCNLanguageProvider()
            : base(LanguageCode.zh_CN, "Tour")
        {
        }

        protected override System.Type GetResourceKindType()
        {
            return typeof(global::AtomUI.Desktop.Controls.Localization.TourLangResourceKind);
        }

        public override void BuildResourceDictionary(IResourceDictionary dictionary)
        {
            var resourceKindType = GetResourceKindType();
            try
            {
                dictionary[global::AtomUI.Desktop.Controls.Localization.TourLangResourceKind.Finish] = global::AtomUI.Desktop.Controls.TourLang.zh_CN.Finish;
                dictionary[global::AtomUI.Desktop.Controls.Localization.TourLangResourceKind.Next] = global::AtomUI.Desktop.Controls.TourLang.zh_CN.Next;
                dictionary[global::AtomUI.Desktop.Controls.Localization.TourLangResourceKind.Previous] = global::AtomUI.Desktop.Controls.TourLang.zh_CN.Previous;
            }
            catch (System.Exception)
            {
                LogBuildResourceDictionaryError(resourceKindType);
                throw;
            }
        }
    }

    internal sealed class TourZhTWLanguageProvider : LanguageProvider
    {
        public TourZhTWLanguageProvider()
            : base(LanguageCode.zh_TW, "Tour")
        {
        }

        protected override System.Type GetResourceKindType()
        {
            return typeof(global::AtomUI.Desktop.Controls.Localization.TourLangResourceKind);
        }

        public override void BuildResourceDictionary(IResourceDictionary dictionary)
        {
            var resourceKindType = GetResourceKindType();
            try
            {
                dictionary[global::AtomUI.Desktop.Controls.Localization.TourLangResourceKind.Finish] = global::AtomUI.Desktop.Controls.TourLang.zh_TW.Finish;
                dictionary[global::AtomUI.Desktop.Controls.Localization.TourLangResourceKind.Next] = global::AtomUI.Desktop.Controls.TourLang.zh_TW.Next;
                dictionary[global::AtomUI.Desktop.Controls.Localization.TourLangResourceKind.Previous] = global::AtomUI.Desktop.Controls.TourLang.zh_TW.Previous;
            }
            catch (System.Exception)
            {
                LogBuildResourceDictionaryError(resourceKindType);
                throw;
            }
        }
    }

    internal sealed class TransferEnUSLanguageProvider : LanguageProvider
    {
        public TransferEnUSLanguageProvider()
            : base(LanguageCode.en_US, "Transfer")
        {
        }

        protected override System.Type GetResourceKindType()
        {
            return typeof(global::AtomUI.Desktop.Controls.Localization.TransferLangResourceKind);
        }

        public override void BuildResourceDictionary(IResourceDictionary dictionary)
        {
            var resourceKindType = GetResourceKindType();
            try
            {
                dictionary[global::AtomUI.Desktop.Controls.Localization.TransferLangResourceKind.DeSelectAll] = global::AtomUI.Desktop.Controls.TransferLang.en_US.DeSelectAll;
                dictionary[global::AtomUI.Desktop.Controls.Localization.TransferLangResourceKind.InvertSelectCurrentPage] = global::AtomUI.Desktop.Controls.TransferLang.en_US.InvertSelectCurrentPage;
                dictionary[global::AtomUI.Desktop.Controls.Localization.TransferLangResourceKind.Item] = global::AtomUI.Desktop.Controls.TransferLang.en_US.Item;
                dictionary[global::AtomUI.Desktop.Controls.Localization.TransferLangResourceKind.Items] = global::AtomUI.Desktop.Controls.TransferLang.en_US.Items;
                dictionary[global::AtomUI.Desktop.Controls.Localization.TransferLangResourceKind.RemoveAll] = global::AtomUI.Desktop.Controls.TransferLang.en_US.RemoveAll;
                dictionary[global::AtomUI.Desktop.Controls.Localization.TransferLangResourceKind.RemoveCurrentPage] = global::AtomUI.Desktop.Controls.TransferLang.en_US.RemoveCurrentPage;
                dictionary[global::AtomUI.Desktop.Controls.Localization.TransferLangResourceKind.SelectAll] = global::AtomUI.Desktop.Controls.TransferLang.en_US.SelectAll;
                dictionary[global::AtomUI.Desktop.Controls.Localization.TransferLangResourceKind.SelectCurrentPage] = global::AtomUI.Desktop.Controls.TransferLang.en_US.SelectCurrentPage;
            }
            catch (System.Exception)
            {
                LogBuildResourceDictionaryError(resourceKindType);
                throw;
            }
        }
    }

    internal sealed class TransferZhCNLanguageProvider : LanguageProvider
    {
        public TransferZhCNLanguageProvider()
            : base(LanguageCode.zh_CN, "Transfer")
        {
        }

        protected override System.Type GetResourceKindType()
        {
            return typeof(global::AtomUI.Desktop.Controls.Localization.TransferLangResourceKind);
        }

        public override void BuildResourceDictionary(IResourceDictionary dictionary)
        {
            var resourceKindType = GetResourceKindType();
            try
            {
                dictionary[global::AtomUI.Desktop.Controls.Localization.TransferLangResourceKind.DeSelectAll] = global::AtomUI.Desktop.Controls.TransferLang.zh_CN.DeSelectAll;
                dictionary[global::AtomUI.Desktop.Controls.Localization.TransferLangResourceKind.InvertSelectCurrentPage] = global::AtomUI.Desktop.Controls.TransferLang.zh_CN.InvertSelectCurrentPage;
                dictionary[global::AtomUI.Desktop.Controls.Localization.TransferLangResourceKind.Item] = global::AtomUI.Desktop.Controls.TransferLang.zh_CN.Item;
                dictionary[global::AtomUI.Desktop.Controls.Localization.TransferLangResourceKind.Items] = global::AtomUI.Desktop.Controls.TransferLang.zh_CN.Items;
                dictionary[global::AtomUI.Desktop.Controls.Localization.TransferLangResourceKind.RemoveAll] = global::AtomUI.Desktop.Controls.TransferLang.zh_CN.RemoveAll;
                dictionary[global::AtomUI.Desktop.Controls.Localization.TransferLangResourceKind.RemoveCurrentPage] = global::AtomUI.Desktop.Controls.TransferLang.zh_CN.RemoveCurrentPage;
                dictionary[global::AtomUI.Desktop.Controls.Localization.TransferLangResourceKind.SelectAll] = global::AtomUI.Desktop.Controls.TransferLang.zh_CN.SelectAll;
                dictionary[global::AtomUI.Desktop.Controls.Localization.TransferLangResourceKind.SelectCurrentPage] = global::AtomUI.Desktop.Controls.TransferLang.zh_CN.SelectCurrentPage;
            }
            catch (System.Exception)
            {
                LogBuildResourceDictionaryError(resourceKindType);
                throw;
            }
        }
    }

    internal sealed class TransferZhTWLanguageProvider : LanguageProvider
    {
        public TransferZhTWLanguageProvider()
            : base(LanguageCode.zh_TW, "Transfer")
        {
        }

        protected override System.Type GetResourceKindType()
        {
            return typeof(global::AtomUI.Desktop.Controls.Localization.TransferLangResourceKind);
        }

        public override void BuildResourceDictionary(IResourceDictionary dictionary)
        {
            var resourceKindType = GetResourceKindType();
            try
            {
                dictionary[global::AtomUI.Desktop.Controls.Localization.TransferLangResourceKind.DeSelectAll] = global::AtomUI.Desktop.Controls.TransferLang.zh_TW.DeSelectAll;
                dictionary[global::AtomUI.Desktop.Controls.Localization.TransferLangResourceKind.InvertSelectCurrentPage] = global::AtomUI.Desktop.Controls.TransferLang.zh_TW.InvertSelectCurrentPage;
                dictionary[global::AtomUI.Desktop.Controls.Localization.TransferLangResourceKind.Item] = global::AtomUI.Desktop.Controls.TransferLang.zh_TW.Item;
                dictionary[global::AtomUI.Desktop.Controls.Localization.TransferLangResourceKind.Items] = global::AtomUI.Desktop.Controls.TransferLang.zh_TW.Items;
                dictionary[global::AtomUI.Desktop.Controls.Localization.TransferLangResourceKind.RemoveAll] = global::AtomUI.Desktop.Controls.TransferLang.zh_TW.RemoveAll;
                dictionary[global::AtomUI.Desktop.Controls.Localization.TransferLangResourceKind.RemoveCurrentPage] = global::AtomUI.Desktop.Controls.TransferLang.zh_TW.RemoveCurrentPage;
                dictionary[global::AtomUI.Desktop.Controls.Localization.TransferLangResourceKind.SelectAll] = global::AtomUI.Desktop.Controls.TransferLang.zh_TW.SelectAll;
                dictionary[global::AtomUI.Desktop.Controls.Localization.TransferLangResourceKind.SelectCurrentPage] = global::AtomUI.Desktop.Controls.TransferLang.zh_TW.SelectCurrentPage;
            }
            catch (System.Exception)
            {
                LogBuildResourceDictionaryError(resourceKindType);
                throw;
            }
        }
    }

    internal sealed class UploadEnUSLanguageProvider : LanguageProvider
    {
        public UploadEnUSLanguageProvider()
            : base(LanguageCode.en_US, "Upload")
        {
        }

        protected override System.Type GetResourceKindType()
        {
            return typeof(global::AtomUI.Desktop.Controls.Localization.UploadLangResourceKind);
        }

        public override void BuildResourceDictionary(IResourceDictionary dictionary)
        {
            var resourceKindType = GetResourceKindType();
            try
            {
                dictionary[global::AtomUI.Desktop.Controls.Localization.UploadLangResourceKind.DragUploadHead] = global::AtomUI.Desktop.Controls.UploadLang.en_US.DragUploadHead;
                dictionary[global::AtomUI.Desktop.Controls.Localization.UploadLangResourceKind.Pending] = global::AtomUI.Desktop.Controls.UploadLang.en_US.Pending;
                dictionary[global::AtomUI.Desktop.Controls.Localization.UploadLangResourceKind.Uploading] = global::AtomUI.Desktop.Controls.UploadLang.en_US.Uploading;
            }
            catch (System.Exception)
            {
                LogBuildResourceDictionaryError(resourceKindType);
                throw;
            }
        }
    }

    internal sealed class UploadZhCNLanguageProvider : LanguageProvider
    {
        public UploadZhCNLanguageProvider()
            : base(LanguageCode.zh_CN, "Upload")
        {
        }

        protected override System.Type GetResourceKindType()
        {
            return typeof(global::AtomUI.Desktop.Controls.Localization.UploadLangResourceKind);
        }

        public override void BuildResourceDictionary(IResourceDictionary dictionary)
        {
            var resourceKindType = GetResourceKindType();
            try
            {
                dictionary[global::AtomUI.Desktop.Controls.Localization.UploadLangResourceKind.DragUploadHead] = global::AtomUI.Desktop.Controls.UploadLang.zh_CN.DragUploadHead;
                dictionary[global::AtomUI.Desktop.Controls.Localization.UploadLangResourceKind.Pending] = global::AtomUI.Desktop.Controls.UploadLang.zh_CN.Pending;
                dictionary[global::AtomUI.Desktop.Controls.Localization.UploadLangResourceKind.Uploading] = global::AtomUI.Desktop.Controls.UploadLang.zh_CN.Uploading;
            }
            catch (System.Exception)
            {
                LogBuildResourceDictionaryError(resourceKindType);
                throw;
            }
        }
    }

    internal sealed class UploadZhTWLanguageProvider : LanguageProvider
    {
        public UploadZhTWLanguageProvider()
            : base(LanguageCode.zh_TW, "Upload")
        {
        }

        protected override System.Type GetResourceKindType()
        {
            return typeof(global::AtomUI.Desktop.Controls.Localization.UploadLangResourceKind);
        }

        public override void BuildResourceDictionary(IResourceDictionary dictionary)
        {
            var resourceKindType = GetResourceKindType();
            try
            {
                dictionary[global::AtomUI.Desktop.Controls.Localization.UploadLangResourceKind.DragUploadHead] = global::AtomUI.Desktop.Controls.UploadLang.zh_TW.DragUploadHead;
                dictionary[global::AtomUI.Desktop.Controls.Localization.UploadLangResourceKind.Pending] = global::AtomUI.Desktop.Controls.UploadLang.zh_TW.Pending;
                dictionary[global::AtomUI.Desktop.Controls.Localization.UploadLangResourceKind.Uploading] = global::AtomUI.Desktop.Controls.UploadLang.zh_TW.Uploading;
            }
            catch (System.Exception)
            {
                LogBuildResourceDictionaryError(resourceKindType);
                throw;
            }
        }
    }

    internal class LanguageProviderPool
    {
        internal static IList<LanguageProvider> GetLanguageProviders()
        {
            List<LanguageProvider> languageProviders = new List<LanguageProvider>(27);
            languageProviders.Add(new DatePickerEnUSLanguageProvider());
            languageProviders.Add(new DatePickerZhCNLanguageProvider());
            languageProviders.Add(new DatePickerZhTWLanguageProvider());
            languageProviders.Add(new DialogEnUSLanguageProvider());
            languageProviders.Add(new DialogZhCNLanguageProvider());
            languageProviders.Add(new DialogZhTWLanguageProvider());
            languageProviders.Add(new ImagePreviewerEnUSLanguageProvider());
            languageProviders.Add(new ImagePreviewerZhCNLanguageProvider());
            languageProviders.Add(new ImagePreviewerZhTWLanguageProvider());
            languageProviders.Add(new PaginationEnUSLanguageProvider());
            languageProviders.Add(new PaginationZhCNLanguageProvider());
            languageProviders.Add(new PaginationZhTWLanguageProvider());
            languageProviders.Add(new QRCodeEnUSLanguageProvider());
            languageProviders.Add(new QRCodeZhCNLanguageProvider());
            languageProviders.Add(new QRCodeZhTWLanguageProvider());
            languageProviders.Add(new TimePickerEnUSLanguageProvider());
            languageProviders.Add(new TimePickerZhCNLanguageProvider());
            languageProviders.Add(new TimePickerZhTWLanguageProvider());
            languageProviders.Add(new TourEnUSLanguageProvider());
            languageProviders.Add(new TourZhCNLanguageProvider());
            languageProviders.Add(new TourZhTWLanguageProvider());
            languageProviders.Add(new TransferEnUSLanguageProvider());
            languageProviders.Add(new TransferZhCNLanguageProvider());
            languageProviders.Add(new TransferZhTWLanguageProvider());
            languageProviders.Add(new UploadEnUSLanguageProvider());
            languageProviders.Add(new UploadZhCNLanguageProvider());
            languageProviders.Add(new UploadZhTWLanguageProvider());
            return languageProviders;
        }
    }
}
