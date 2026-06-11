// This code is auto generated. Do not modify.
using System;
using System.Diagnostics.CodeAnalysis;
using AtomUI.Controls;

namespace AtomUI.Icons.AntDesign;

public partial class AntDesignIconProvider
{
    private const int IconFactoryIconCount = 843;
    private const int IconFactoryChunkSize = 64;

    private static int GetIconIndex(AntDesignIconKind kind)
    {
        var index = (int)kind - 1;
        if ((uint)index >= IconFactoryIconCount)
        {
            throw new InvalidOperationException($"Icon kind {kind} does not exist");
        }

        return index;
    }

    [UnconditionalSuppressMessage("Trimming", "IL2063",
        Justification = "Every switch arm returns typeof(...) for a generated icon class with a public parameterless constructor.")]
    [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
    private static Type GetIconType(AntDesignIconKind kind)
    {
        var index = GetIconIndex(kind);
        return GetIconTypeChunk(index / IconFactoryChunkSize, kind);
    }

    [UnconditionalSuppressMessage("Trimming", "IL2063",
        Justification = "Every switch arm returns typeof(...) for a generated icon class with a public parameterless constructor.")]
    [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
    private static Type GetIconTypeChunk(int chunkIndex, AntDesignIconKind kind)
    {
        return chunkIndex switch
        {
            0 => GetIconTypeChunk0(kind),
            1 => GetIconTypeChunk1(kind),
            2 => GetIconTypeChunk2(kind),
            3 => GetIconTypeChunk3(kind),
            4 => GetIconTypeChunk4(kind),
            5 => GetIconTypeChunk5(kind),
            6 => GetIconTypeChunk6(kind),
            7 => GetIconTypeChunk7(kind),
            8 => GetIconTypeChunk8(kind),
            9 => GetIconTypeChunk9(kind),
            10 => GetIconTypeChunk10(kind),
            11 => GetIconTypeChunk11(kind),
            12 => GetIconTypeChunk12(kind),
            13 => GetIconTypeChunk13(kind),
            _ => throw new ArgumentOutOfRangeException(nameof(chunkIndex))
        };
    }

    private static Icon CreateIcon(AntDesignIconKind kind)
    {
        var index = GetIconIndex(kind);
        return CreateIconChunk(index / IconFactoryChunkSize, kind);
    }

    private static Icon CreateIconChunk(int chunkIndex, AntDesignIconKind kind)
    {
        return chunkIndex switch
        {
            0 => CreateIconChunk0(kind),
            1 => CreateIconChunk1(kind),
            2 => CreateIconChunk2(kind),
            3 => CreateIconChunk3(kind),
            4 => CreateIconChunk4(kind),
            5 => CreateIconChunk5(kind),
            6 => CreateIconChunk6(kind),
            7 => CreateIconChunk7(kind),
            8 => CreateIconChunk8(kind),
            9 => CreateIconChunk9(kind),
            10 => CreateIconChunk10(kind),
            11 => CreateIconChunk11(kind),
            12 => CreateIconChunk12(kind),
            13 => CreateIconChunk13(kind),
            _ => throw new ArgumentOutOfRangeException(nameof(chunkIndex))
        };
    }

    [UnconditionalSuppressMessage("Trimming", "IL2063",
        Justification = "Every switch arm returns typeof(...) for a generated icon class with a public parameterless constructor.")]
    [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
    private static Type GetIconTypeChunk0(AntDesignIconKind kind)
    {
        switch (kind)
        {
            case AntDesignIconKind.LikeTwoTone: return typeof(LikeTwoTone);
            case AntDesignIconKind.PauseCircleTwoTone: return typeof(PauseCircleTwoTone);
            case AntDesignIconKind.CheckSquareTwoTone: return typeof(CheckSquareTwoTone);
            case AntDesignIconKind.SwitcherTwoTone: return typeof(SwitcherTwoTone);
            case AntDesignIconKind.MoneyCollectTwoTone: return typeof(MoneyCollectTwoTone);
            case AntDesignIconKind.BulbTwoTone: return typeof(BulbTwoTone);
            case AntDesignIconKind.FileUnknownTwoTone: return typeof(FileUnknownTwoTone);
            case AntDesignIconKind.AppstoreTwoTone: return typeof(AppstoreTwoTone);
            case AntDesignIconKind.FileExcelTwoTone: return typeof(FileExcelTwoTone);
            case AntDesignIconKind.SoundTwoTone: return typeof(SoundTwoTone);
            case AntDesignIconKind.LeftCircleTwoTone: return typeof(LeftCircleTwoTone);
            case AntDesignIconKind.PlayCircleTwoTone: return typeof(PlayCircleTwoTone);
            case AntDesignIconKind.FileZipTwoTone: return typeof(FileZipTwoTone);
            case AntDesignIconKind.HourglassTwoTone: return typeof(HourglassTwoTone);
            case AntDesignIconKind.HighlightTwoTone: return typeof(HighlightTwoTone);
            case AntDesignIconKind.ReconciliationTwoTone: return typeof(ReconciliationTwoTone);
            case AntDesignIconKind.DollarTwoTone: return typeof(DollarTwoTone);
            case AntDesignIconKind.HomeTwoTone: return typeof(HomeTwoTone);
            case AntDesignIconKind.PoundCircleTwoTone: return typeof(PoundCircleTwoTone);
            case AntDesignIconKind.ShopTwoTone: return typeof(ShopTwoTone);
            case AntDesignIconKind.CopyrightTwoTone: return typeof(CopyrightTwoTone);
            case AntDesignIconKind.AlertTwoTone: return typeof(AlertTwoTone);
            case AntDesignIconKind.SlidersTwoTone: return typeof(SlidersTwoTone);
            case AntDesignIconKind.DollarCircleTwoTone: return typeof(DollarCircleTwoTone);
            case AntDesignIconKind.ShoppingTwoTone: return typeof(ShoppingTwoTone);
            case AntDesignIconKind.FileWordTwoTone: return typeof(FileWordTwoTone);
            case AntDesignIconKind.FunnelPlotTwoTone: return typeof(FunnelPlotTwoTone);
            case AntDesignIconKind.UsbTwoTone: return typeof(UsbTwoTone);
            case AntDesignIconKind.EuroCircleTwoTone: return typeof(EuroCircleTwoTone);
            case AntDesignIconKind.TagTwoTone: return typeof(TagTwoTone);
            case AntDesignIconKind.UpSquareTwoTone: return typeof(UpSquareTwoTone);
            case AntDesignIconKind.DownSquareTwoTone: return typeof(DownSquareTwoTone);
            case AntDesignIconKind.FileAddTwoTone: return typeof(FileAddTwoTone);
            case AntDesignIconKind.PlusSquareTwoTone: return typeof(PlusSquareTwoTone);
            case AntDesignIconKind.DatabaseTwoTone: return typeof(DatabaseTwoTone);
            case AntDesignIconKind.FileTwoTone: return typeof(FileTwoTone);
            case AntDesignIconKind.AccountBookTwoTone: return typeof(AccountBookTwoTone);
            case AntDesignIconKind.ControlTwoTone: return typeof(ControlTwoTone);
            case AntDesignIconKind.RedEnvelopeTwoTone: return typeof(RedEnvelopeTwoTone);
            case AntDesignIconKind.BoxPlotTwoTone: return typeof(BoxPlotTwoTone);
            case AntDesignIconKind.FileTextTwoTone: return typeof(FileTextTwoTone);
            case AntDesignIconKind.FolderOpenTwoTone: return typeof(FolderOpenTwoTone);
            case AntDesignIconKind.BuildTwoTone: return typeof(BuildTwoTone);
            case AntDesignIconKind.QuestionCircleTwoTone: return typeof(QuestionCircleTwoTone);
            case AntDesignIconKind.LockTwoTone: return typeof(LockTwoTone);
            case AntDesignIconKind.FireTwoTone: return typeof(FireTwoTone);
            case AntDesignIconKind.DislikeTwoTone: return typeof(DislikeTwoTone);
            case AntDesignIconKind.EuroTwoTone: return typeof(EuroTwoTone);
            case AntDesignIconKind.IdcardTwoTone: return typeof(IdcardTwoTone);
            case AntDesignIconKind.MehTwoTone: return typeof(MehTwoTone);
            case AntDesignIconKind.CiTwoTone: return typeof(CiTwoTone);
            case AntDesignIconKind.DiffTwoTone: return typeof(DiffTwoTone);
            case AntDesignIconKind.MinusSquareTwoTone: return typeof(MinusSquareTwoTone);
            case AntDesignIconKind.CloseCircleTwoTone: return typeof(CloseCircleTwoTone);
            case AntDesignIconKind.MailTwoTone: return typeof(MailTwoTone);
            case AntDesignIconKind.BookTwoTone: return typeof(BookTwoTone);
            case AntDesignIconKind.WalletTwoTone: return typeof(WalletTwoTone);
            case AntDesignIconKind.FileImageTwoTone: return typeof(FileImageTwoTone);
            case AntDesignIconKind.BellTwoTone: return typeof(BellTwoTone);
            case AntDesignIconKind.DashboardTwoTone: return typeof(DashboardTwoTone);
            case AntDesignIconKind.CodeTwoTone: return typeof(CodeTwoTone);
            case AntDesignIconKind.CarryOutTwoTone: return typeof(CarryOutTwoTone);
            case AntDesignIconKind.FlagTwoTone: return typeof(FlagTwoTone);
            case AntDesignIconKind.SnippetsTwoTone: return typeof(SnippetsTwoTone);
            default: throw new InvalidOperationException($"Icon kind {kind} does not exist");
        }
    }

    private static Icon CreateIconChunk0(AntDesignIconKind kind)
    {
        return kind switch
        {
            AntDesignIconKind.LikeTwoTone => new LikeTwoTone(),
            AntDesignIconKind.PauseCircleTwoTone => new PauseCircleTwoTone(),
            AntDesignIconKind.CheckSquareTwoTone => new CheckSquareTwoTone(),
            AntDesignIconKind.SwitcherTwoTone => new SwitcherTwoTone(),
            AntDesignIconKind.MoneyCollectTwoTone => new MoneyCollectTwoTone(),
            AntDesignIconKind.BulbTwoTone => new BulbTwoTone(),
            AntDesignIconKind.FileUnknownTwoTone => new FileUnknownTwoTone(),
            AntDesignIconKind.AppstoreTwoTone => new AppstoreTwoTone(),
            AntDesignIconKind.FileExcelTwoTone => new FileExcelTwoTone(),
            AntDesignIconKind.SoundTwoTone => new SoundTwoTone(),
            AntDesignIconKind.LeftCircleTwoTone => new LeftCircleTwoTone(),
            AntDesignIconKind.PlayCircleTwoTone => new PlayCircleTwoTone(),
            AntDesignIconKind.FileZipTwoTone => new FileZipTwoTone(),
            AntDesignIconKind.HourglassTwoTone => new HourglassTwoTone(),
            AntDesignIconKind.HighlightTwoTone => new HighlightTwoTone(),
            AntDesignIconKind.ReconciliationTwoTone => new ReconciliationTwoTone(),
            AntDesignIconKind.DollarTwoTone => new DollarTwoTone(),
            AntDesignIconKind.HomeTwoTone => new HomeTwoTone(),
            AntDesignIconKind.PoundCircleTwoTone => new PoundCircleTwoTone(),
            AntDesignIconKind.ShopTwoTone => new ShopTwoTone(),
            AntDesignIconKind.CopyrightTwoTone => new CopyrightTwoTone(),
            AntDesignIconKind.AlertTwoTone => new AlertTwoTone(),
            AntDesignIconKind.SlidersTwoTone => new SlidersTwoTone(),
            AntDesignIconKind.DollarCircleTwoTone => new DollarCircleTwoTone(),
            AntDesignIconKind.ShoppingTwoTone => new ShoppingTwoTone(),
            AntDesignIconKind.FileWordTwoTone => new FileWordTwoTone(),
            AntDesignIconKind.FunnelPlotTwoTone => new FunnelPlotTwoTone(),
            AntDesignIconKind.UsbTwoTone => new UsbTwoTone(),
            AntDesignIconKind.EuroCircleTwoTone => new EuroCircleTwoTone(),
            AntDesignIconKind.TagTwoTone => new TagTwoTone(),
            AntDesignIconKind.UpSquareTwoTone => new UpSquareTwoTone(),
            AntDesignIconKind.DownSquareTwoTone => new DownSquareTwoTone(),
            AntDesignIconKind.FileAddTwoTone => new FileAddTwoTone(),
            AntDesignIconKind.PlusSquareTwoTone => new PlusSquareTwoTone(),
            AntDesignIconKind.DatabaseTwoTone => new DatabaseTwoTone(),
            AntDesignIconKind.FileTwoTone => new FileTwoTone(),
            AntDesignIconKind.AccountBookTwoTone => new AccountBookTwoTone(),
            AntDesignIconKind.ControlTwoTone => new ControlTwoTone(),
            AntDesignIconKind.RedEnvelopeTwoTone => new RedEnvelopeTwoTone(),
            AntDesignIconKind.BoxPlotTwoTone => new BoxPlotTwoTone(),
            AntDesignIconKind.FileTextTwoTone => new FileTextTwoTone(),
            AntDesignIconKind.FolderOpenTwoTone => new FolderOpenTwoTone(),
            AntDesignIconKind.BuildTwoTone => new BuildTwoTone(),
            AntDesignIconKind.QuestionCircleTwoTone => new QuestionCircleTwoTone(),
            AntDesignIconKind.LockTwoTone => new LockTwoTone(),
            AntDesignIconKind.FireTwoTone => new FireTwoTone(),
            AntDesignIconKind.DislikeTwoTone => new DislikeTwoTone(),
            AntDesignIconKind.EuroTwoTone => new EuroTwoTone(),
            AntDesignIconKind.IdcardTwoTone => new IdcardTwoTone(),
            AntDesignIconKind.MehTwoTone => new MehTwoTone(),
            AntDesignIconKind.CiTwoTone => new CiTwoTone(),
            AntDesignIconKind.DiffTwoTone => new DiffTwoTone(),
            AntDesignIconKind.MinusSquareTwoTone => new MinusSquareTwoTone(),
            AntDesignIconKind.CloseCircleTwoTone => new CloseCircleTwoTone(),
            AntDesignIconKind.MailTwoTone => new MailTwoTone(),
            AntDesignIconKind.BookTwoTone => new BookTwoTone(),
            AntDesignIconKind.WalletTwoTone => new WalletTwoTone(),
            AntDesignIconKind.FileImageTwoTone => new FileImageTwoTone(),
            AntDesignIconKind.BellTwoTone => new BellTwoTone(),
            AntDesignIconKind.DashboardTwoTone => new DashboardTwoTone(),
            AntDesignIconKind.CodeTwoTone => new CodeTwoTone(),
            AntDesignIconKind.CarryOutTwoTone => new CarryOutTwoTone(),
            AntDesignIconKind.FlagTwoTone => new FlagTwoTone(),
            AntDesignIconKind.SnippetsTwoTone => new SnippetsTwoTone(),
            _ => throw new InvalidOperationException($"Icon kind {kind} does not exist")
        };
    }

    [UnconditionalSuppressMessage("Trimming", "IL2063",
        Justification = "Every switch arm returns typeof(...) for a generated icon class with a public parameterless constructor.")]
    [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
    private static Type GetIconTypeChunk1(AntDesignIconKind kind)
    {
        switch (kind)
        {
            case AntDesignIconKind.StopTwoTone: return typeof(StopTwoTone);
            case AntDesignIconKind.RightCircleTwoTone: return typeof(RightCircleTwoTone);
            case AntDesignIconKind.ContainerTwoTone: return typeof(ContainerTwoTone);
            case AntDesignIconKind.FrownTwoTone: return typeof(FrownTwoTone);
            case AntDesignIconKind.ToolTwoTone: return typeof(ToolTwoTone);
            case AntDesignIconKind.SafetyCertificateTwoTone: return typeof(SafetyCertificateTwoTone);
            case AntDesignIconKind.TrophyTwoTone: return typeof(TrophyTwoTone);
            case AntDesignIconKind.WarningTwoTone: return typeof(WarningTwoTone);
            case AntDesignIconKind.PieChartTwoTone: return typeof(PieChartTwoTone);
            case AntDesignIconKind.SecurityScanTwoTone: return typeof(SecurityScanTwoTone);
            case AntDesignIconKind.InfoCircleTwoTone: return typeof(InfoCircleTwoTone);
            case AntDesignIconKind.EyeInvisibleTwoTone: return typeof(EyeInvisibleTwoTone);
            case AntDesignIconKind.LeftSquareTwoTone: return typeof(LeftSquareTwoTone);
            case AntDesignIconKind.CopyTwoTone: return typeof(CopyTwoTone);
            case AntDesignIconKind.GoldTwoTone: return typeof(GoldTwoTone);
            case AntDesignIconKind.FundTwoTone: return typeof(FundTwoTone);
            case AntDesignIconKind.PlaySquareTwoTone: return typeof(PlaySquareTwoTone);
            case AntDesignIconKind.FileExclamationTwoTone: return typeof(FileExclamationTwoTone);
            case AntDesignIconKind.EnvironmentTwoTone: return typeof(EnvironmentTwoTone);
            case AntDesignIconKind.CheckCircleTwoTone: return typeof(CheckCircleTwoTone);
            case AntDesignIconKind.Html5TwoTone: return typeof(Html5TwoTone);
            case AntDesignIconKind.SaveTwoTone: return typeof(SaveTwoTone);
            case AntDesignIconKind.SmileTwoTone: return typeof(SmileTwoTone);
            case AntDesignIconKind.SettingTwoTone: return typeof(SettingTwoTone);
            case AntDesignIconKind.MessageTwoTone: return typeof(MessageTwoTone);
            case AntDesignIconKind.CopyrightCircleTwoTone: return typeof(CopyrightCircleTwoTone);
            case AntDesignIconKind.CrownTwoTone: return typeof(CrownTwoTone);
            case AntDesignIconKind.NotificationTwoTone: return typeof(NotificationTwoTone);
            case AntDesignIconKind.PictureTwoTone: return typeof(PictureTwoTone);
            case AntDesignIconKind.CameraTwoTone: return typeof(CameraTwoTone);
            case AntDesignIconKind.PrinterTwoTone: return typeof(PrinterTwoTone);
            case AntDesignIconKind.UpCircleTwoTone: return typeof(UpCircleTwoTone);
            case AntDesignIconKind.ExclamationCircleTwoTone: return typeof(ExclamationCircleTwoTone);
            case AntDesignIconKind.DownCircleTwoTone: return typeof(DownCircleTwoTone);
            case AntDesignIconKind.RestTwoTone: return typeof(RestTwoTone);
            case AntDesignIconKind.ContactsTwoTone: return typeof(ContactsTwoTone);
            case AntDesignIconKind.StarTwoTone: return typeof(StarTwoTone);
            case AntDesignIconKind.TrademarkCircleTwoTone: return typeof(TrademarkCircleTwoTone);
            case AntDesignIconKind.ExperimentTwoTone: return typeof(ExperimentTwoTone);
            case AntDesignIconKind.EditTwoTone: return typeof(EditTwoTone);
            case AntDesignIconKind.ApiTwoTone: return typeof(ApiTwoTone);
            case AntDesignIconKind.BugTwoTone: return typeof(BugTwoTone);
            case AntDesignIconKind.UnlockTwoTone: return typeof(UnlockTwoTone);
            case AntDesignIconKind.CompassTwoTone: return typeof(CompassTwoTone);
            case AntDesignIconKind.PlusCircleTwoTone: return typeof(PlusCircleTwoTone);
            case AntDesignIconKind.BankTwoTone: return typeof(BankTwoTone);
            case AntDesignIconKind.CreditCardTwoTone: return typeof(CreditCardTwoTone);
            case AntDesignIconKind.FileMarkdownTwoTone: return typeof(FileMarkdownTwoTone);
            case AntDesignIconKind.AudioTwoTone: return typeof(AudioTwoTone);
            case AntDesignIconKind.DeleteTwoTone: return typeof(DeleteTwoTone);
            case AntDesignIconKind.SkinTwoTone: return typeof(SkinTwoTone);
            case AntDesignIconKind.PhoneTwoTone: return typeof(PhoneTwoTone);
            case AntDesignIconKind.EyeTwoTone: return typeof(EyeTwoTone);
            case AntDesignIconKind.MobileTwoTone: return typeof(MobileTwoTone);
            case AntDesignIconKind.InsuranceTwoTone: return typeof(InsuranceTwoTone);
            case AntDesignIconKind.GiftTwoTone: return typeof(GiftTwoTone);
            case AntDesignIconKind.CarTwoTone: return typeof(CarTwoTone);
            case AntDesignIconKind.CiCircleTwoTone: return typeof(CiCircleTwoTone);
            case AntDesignIconKind.ThunderboltTwoTone: return typeof(ThunderboltTwoTone);
            case AntDesignIconKind.ProfileTwoTone: return typeof(ProfileTwoTone);
            case AntDesignIconKind.TagsTwoTone: return typeof(TagsTwoTone);
            case AntDesignIconKind.FolderAddTwoTone: return typeof(FolderAddTwoTone);
            case AntDesignIconKind.ScheduleTwoTone: return typeof(ScheduleTwoTone);
            case AntDesignIconKind.FilterTwoTone: return typeof(FilterTwoTone);
            default: throw new InvalidOperationException($"Icon kind {kind} does not exist");
        }
    }

    private static Icon CreateIconChunk1(AntDesignIconKind kind)
    {
        return kind switch
        {
            AntDesignIconKind.StopTwoTone => new StopTwoTone(),
            AntDesignIconKind.RightCircleTwoTone => new RightCircleTwoTone(),
            AntDesignIconKind.ContainerTwoTone => new ContainerTwoTone(),
            AntDesignIconKind.FrownTwoTone => new FrownTwoTone(),
            AntDesignIconKind.ToolTwoTone => new ToolTwoTone(),
            AntDesignIconKind.SafetyCertificateTwoTone => new SafetyCertificateTwoTone(),
            AntDesignIconKind.TrophyTwoTone => new TrophyTwoTone(),
            AntDesignIconKind.WarningTwoTone => new WarningTwoTone(),
            AntDesignIconKind.PieChartTwoTone => new PieChartTwoTone(),
            AntDesignIconKind.SecurityScanTwoTone => new SecurityScanTwoTone(),
            AntDesignIconKind.InfoCircleTwoTone => new InfoCircleTwoTone(),
            AntDesignIconKind.EyeInvisibleTwoTone => new EyeInvisibleTwoTone(),
            AntDesignIconKind.LeftSquareTwoTone => new LeftSquareTwoTone(),
            AntDesignIconKind.CopyTwoTone => new CopyTwoTone(),
            AntDesignIconKind.GoldTwoTone => new GoldTwoTone(),
            AntDesignIconKind.FundTwoTone => new FundTwoTone(),
            AntDesignIconKind.PlaySquareTwoTone => new PlaySquareTwoTone(),
            AntDesignIconKind.FileExclamationTwoTone => new FileExclamationTwoTone(),
            AntDesignIconKind.EnvironmentTwoTone => new EnvironmentTwoTone(),
            AntDesignIconKind.CheckCircleTwoTone => new CheckCircleTwoTone(),
            AntDesignIconKind.Html5TwoTone => new Html5TwoTone(),
            AntDesignIconKind.SaveTwoTone => new SaveTwoTone(),
            AntDesignIconKind.SmileTwoTone => new SmileTwoTone(),
            AntDesignIconKind.SettingTwoTone => new SettingTwoTone(),
            AntDesignIconKind.MessageTwoTone => new MessageTwoTone(),
            AntDesignIconKind.CopyrightCircleTwoTone => new CopyrightCircleTwoTone(),
            AntDesignIconKind.CrownTwoTone => new CrownTwoTone(),
            AntDesignIconKind.NotificationTwoTone => new NotificationTwoTone(),
            AntDesignIconKind.PictureTwoTone => new PictureTwoTone(),
            AntDesignIconKind.CameraTwoTone => new CameraTwoTone(),
            AntDesignIconKind.PrinterTwoTone => new PrinterTwoTone(),
            AntDesignIconKind.UpCircleTwoTone => new UpCircleTwoTone(),
            AntDesignIconKind.ExclamationCircleTwoTone => new ExclamationCircleTwoTone(),
            AntDesignIconKind.DownCircleTwoTone => new DownCircleTwoTone(),
            AntDesignIconKind.RestTwoTone => new RestTwoTone(),
            AntDesignIconKind.ContactsTwoTone => new ContactsTwoTone(),
            AntDesignIconKind.StarTwoTone => new StarTwoTone(),
            AntDesignIconKind.TrademarkCircleTwoTone => new TrademarkCircleTwoTone(),
            AntDesignIconKind.ExperimentTwoTone => new ExperimentTwoTone(),
            AntDesignIconKind.EditTwoTone => new EditTwoTone(),
            AntDesignIconKind.ApiTwoTone => new ApiTwoTone(),
            AntDesignIconKind.BugTwoTone => new BugTwoTone(),
            AntDesignIconKind.UnlockTwoTone => new UnlockTwoTone(),
            AntDesignIconKind.CompassTwoTone => new CompassTwoTone(),
            AntDesignIconKind.PlusCircleTwoTone => new PlusCircleTwoTone(),
            AntDesignIconKind.BankTwoTone => new BankTwoTone(),
            AntDesignIconKind.CreditCardTwoTone => new CreditCardTwoTone(),
            AntDesignIconKind.FileMarkdownTwoTone => new FileMarkdownTwoTone(),
            AntDesignIconKind.AudioTwoTone => new AudioTwoTone(),
            AntDesignIconKind.DeleteTwoTone => new DeleteTwoTone(),
            AntDesignIconKind.SkinTwoTone => new SkinTwoTone(),
            AntDesignIconKind.PhoneTwoTone => new PhoneTwoTone(),
            AntDesignIconKind.EyeTwoTone => new EyeTwoTone(),
            AntDesignIconKind.MobileTwoTone => new MobileTwoTone(),
            AntDesignIconKind.InsuranceTwoTone => new InsuranceTwoTone(),
            AntDesignIconKind.GiftTwoTone => new GiftTwoTone(),
            AntDesignIconKind.CarTwoTone => new CarTwoTone(),
            AntDesignIconKind.CiCircleTwoTone => new CiCircleTwoTone(),
            AntDesignIconKind.ThunderboltTwoTone => new ThunderboltTwoTone(),
            AntDesignIconKind.ProfileTwoTone => new ProfileTwoTone(),
            AntDesignIconKind.TagsTwoTone => new TagsTwoTone(),
            AntDesignIconKind.FolderAddTwoTone => new FolderAddTwoTone(),
            AntDesignIconKind.ScheduleTwoTone => new ScheduleTwoTone(),
            AntDesignIconKind.FilterTwoTone => new FilterTwoTone(),
            _ => throw new InvalidOperationException($"Icon kind {kind} does not exist")
        };
    }

    [UnconditionalSuppressMessage("Trimming", "IL2063",
        Justification = "Every switch arm returns typeof(...) for a generated icon class with a public parameterless constructor.")]
    [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
    private static Type GetIconTypeChunk2(AntDesignIconKind kind)
    {
        switch (kind)
        {
            case AntDesignIconKind.CalendarTwoTone: return typeof(CalendarTwoTone);
            case AntDesignIconKind.VideoCameraTwoTone: return typeof(VideoCameraTwoTone);
            case AntDesignIconKind.MinusCircleTwoTone: return typeof(MinusCircleTwoTone);
            case AntDesignIconKind.CloseSquareTwoTone: return typeof(CloseSquareTwoTone);
            case AntDesignIconKind.CloudTwoTone: return typeof(CloudTwoTone);
            case AntDesignIconKind.InteractionTwoTone: return typeof(InteractionTwoTone);
            case AntDesignIconKind.PropertySafetyTwoTone: return typeof(PropertySafetyTwoTone);
            case AntDesignIconKind.RightSquareTwoTone: return typeof(RightSquareTwoTone);
            case AntDesignIconKind.RocketTwoTone: return typeof(RocketTwoTone);
            case AntDesignIconKind.TabletTwoTone: return typeof(TabletTwoTone);
            case AntDesignIconKind.PushpinTwoTone: return typeof(PushpinTwoTone);
            case AntDesignIconKind.HddTwoTone: return typeof(HddTwoTone);
            case AntDesignIconKind.CalculatorTwoTone: return typeof(CalculatorTwoTone);
            case AntDesignIconKind.MedicineBoxTwoTone: return typeof(MedicineBoxTwoTone);
            case AntDesignIconKind.ProjectTwoTone: return typeof(ProjectTwoTone);
            case AntDesignIconKind.FolderTwoTone: return typeof(FolderTwoTone);
            case AntDesignIconKind.FilePptTwoTone: return typeof(FilePptTwoTone);
            case AntDesignIconKind.FilePdfTwoTone: return typeof(FilePdfTwoTone);
            case AntDesignIconKind.CustomerServiceTwoTone: return typeof(CustomerServiceTwoTone);
            case AntDesignIconKind.LayoutTwoTone: return typeof(LayoutTwoTone);
            case AntDesignIconKind.ClockCircleTwoTone: return typeof(ClockCircleTwoTone);
            case AntDesignIconKind.HeartTwoTone: return typeof(HeartTwoTone);
            case AntDesignIconKind.AppstoreAddOutlined: return typeof(AppstoreAddOutlined);
            case AntDesignIconKind.UnderlineOutlined: return typeof(UnderlineOutlined);
            case AntDesignIconKind.SmallDashOutlined: return typeof(SmallDashOutlined);
            case AntDesignIconKind.CaretDownOutlined: return typeof(CaretDownOutlined);
            case AntDesignIconKind.MediumOutlined: return typeof(MediumOutlined);
            case AntDesignIconKind.SearchOutlined: return typeof(SearchOutlined);
            case AntDesignIconKind.VerticalAlignMiddleOutlined: return typeof(VerticalAlignMiddleOutlined);
            case AntDesignIconKind.DeleteRowOutlined: return typeof(DeleteRowOutlined);
            case AntDesignIconKind.LikeOutlined: return typeof(LikeOutlined);
            case AntDesignIconKind.PauseCircleOutlined: return typeof(PauseCircleOutlined);
            case AntDesignIconKind.PicLeftOutlined: return typeof(PicLeftOutlined);
            case AntDesignIconKind.CheckSquareOutlined: return typeof(CheckSquareOutlined);
            case AntDesignIconKind.FileSearchOutlined: return typeof(FileSearchOutlined);
            case AntDesignIconKind.ArrowDownOutlined: return typeof(ArrowDownOutlined);
            case AntDesignIconKind.SwitcherOutlined: return typeof(SwitcherOutlined);
            case AntDesignIconKind.RollbackOutlined: return typeof(RollbackOutlined);
            case AntDesignIconKind.MoneyCollectOutlined: return typeof(MoneyCollectOutlined);
            case AntDesignIconKind.BulbOutlined: return typeof(BulbOutlined);
            case AntDesignIconKind.VerticalLeftOutlined: return typeof(VerticalLeftOutlined);
            case AntDesignIconKind.FileUnknownOutlined: return typeof(FileUnknownOutlined);
            case AntDesignIconKind.AppstoreOutlined: return typeof(AppstoreOutlined);
            case AntDesignIconKind.FileExcelOutlined: return typeof(FileExcelOutlined);
            case AntDesignIconKind.SoundOutlined: return typeof(SoundOutlined);
            case AntDesignIconKind.UnorderedListOutlined: return typeof(UnorderedListOutlined);
            case AntDesignIconKind.RotateLeftOutlined: return typeof(RotateLeftOutlined);
            case AntDesignIconKind.LeftCircleOutlined: return typeof(LeftCircleOutlined);
            case AntDesignIconKind.PlayCircleOutlined: return typeof(PlayCircleOutlined);
            case AntDesignIconKind.FileZipOutlined: return typeof(FileZipOutlined);
            case AntDesignIconKind.BorderOuterOutlined: return typeof(BorderOuterOutlined);
            case AntDesignIconKind.MediumWorkmarkOutlined: return typeof(MediumWorkmarkOutlined);
            case AntDesignIconKind.DropboxOutlined: return typeof(DropboxOutlined);
            case AntDesignIconKind.InsertRowAboveOutlined: return typeof(InsertRowAboveOutlined);
            case AntDesignIconKind.HourglassOutlined: return typeof(HourglassOutlined);
            case AntDesignIconKind.SortDescendingOutlined: return typeof(SortDescendingOutlined);
            case AntDesignIconKind.SwapLeftOutlined: return typeof(SwapLeftOutlined);
            case AntDesignIconKind.SafetyOutlined: return typeof(SafetyOutlined);
            case AntDesignIconKind.MenuUnfoldOutlined: return typeof(MenuUnfoldOutlined);
            case AntDesignIconKind.NodeCollapseOutlined: return typeof(NodeCollapseOutlined);
            case AntDesignIconKind.UserOutlined: return typeof(UserOutlined);
            case AntDesignIconKind.HighlightOutlined: return typeof(HighlightOutlined);
            case AntDesignIconKind.DeliveredProcedureOutlined: return typeof(DeliveredProcedureOutlined);
            case AntDesignIconKind.FullscreenOutlined: return typeof(FullscreenOutlined);
            default: throw new InvalidOperationException($"Icon kind {kind} does not exist");
        }
    }

    private static Icon CreateIconChunk2(AntDesignIconKind kind)
    {
        return kind switch
        {
            AntDesignIconKind.CalendarTwoTone => new CalendarTwoTone(),
            AntDesignIconKind.VideoCameraTwoTone => new VideoCameraTwoTone(),
            AntDesignIconKind.MinusCircleTwoTone => new MinusCircleTwoTone(),
            AntDesignIconKind.CloseSquareTwoTone => new CloseSquareTwoTone(),
            AntDesignIconKind.CloudTwoTone => new CloudTwoTone(),
            AntDesignIconKind.InteractionTwoTone => new InteractionTwoTone(),
            AntDesignIconKind.PropertySafetyTwoTone => new PropertySafetyTwoTone(),
            AntDesignIconKind.RightSquareTwoTone => new RightSquareTwoTone(),
            AntDesignIconKind.RocketTwoTone => new RocketTwoTone(),
            AntDesignIconKind.TabletTwoTone => new TabletTwoTone(),
            AntDesignIconKind.PushpinTwoTone => new PushpinTwoTone(),
            AntDesignIconKind.HddTwoTone => new HddTwoTone(),
            AntDesignIconKind.CalculatorTwoTone => new CalculatorTwoTone(),
            AntDesignIconKind.MedicineBoxTwoTone => new MedicineBoxTwoTone(),
            AntDesignIconKind.ProjectTwoTone => new ProjectTwoTone(),
            AntDesignIconKind.FolderTwoTone => new FolderTwoTone(),
            AntDesignIconKind.FilePptTwoTone => new FilePptTwoTone(),
            AntDesignIconKind.FilePdfTwoTone => new FilePdfTwoTone(),
            AntDesignIconKind.CustomerServiceTwoTone => new CustomerServiceTwoTone(),
            AntDesignIconKind.LayoutTwoTone => new LayoutTwoTone(),
            AntDesignIconKind.ClockCircleTwoTone => new ClockCircleTwoTone(),
            AntDesignIconKind.HeartTwoTone => new HeartTwoTone(),
            AntDesignIconKind.AppstoreAddOutlined => new AppstoreAddOutlined(),
            AntDesignIconKind.UnderlineOutlined => new UnderlineOutlined(),
            AntDesignIconKind.SmallDashOutlined => new SmallDashOutlined(),
            AntDesignIconKind.CaretDownOutlined => new CaretDownOutlined(),
            AntDesignIconKind.MediumOutlined => new MediumOutlined(),
            AntDesignIconKind.SearchOutlined => new SearchOutlined(),
            AntDesignIconKind.VerticalAlignMiddleOutlined => new VerticalAlignMiddleOutlined(),
            AntDesignIconKind.DeleteRowOutlined => new DeleteRowOutlined(),
            AntDesignIconKind.LikeOutlined => new LikeOutlined(),
            AntDesignIconKind.PauseCircleOutlined => new PauseCircleOutlined(),
            AntDesignIconKind.PicLeftOutlined => new PicLeftOutlined(),
            AntDesignIconKind.CheckSquareOutlined => new CheckSquareOutlined(),
            AntDesignIconKind.FileSearchOutlined => new FileSearchOutlined(),
            AntDesignIconKind.ArrowDownOutlined => new ArrowDownOutlined(),
            AntDesignIconKind.SwitcherOutlined => new SwitcherOutlined(),
            AntDesignIconKind.RollbackOutlined => new RollbackOutlined(),
            AntDesignIconKind.MoneyCollectOutlined => new MoneyCollectOutlined(),
            AntDesignIconKind.BulbOutlined => new BulbOutlined(),
            AntDesignIconKind.VerticalLeftOutlined => new VerticalLeftOutlined(),
            AntDesignIconKind.FileUnknownOutlined => new FileUnknownOutlined(),
            AntDesignIconKind.AppstoreOutlined => new AppstoreOutlined(),
            AntDesignIconKind.FileExcelOutlined => new FileExcelOutlined(),
            AntDesignIconKind.SoundOutlined => new SoundOutlined(),
            AntDesignIconKind.UnorderedListOutlined => new UnorderedListOutlined(),
            AntDesignIconKind.RotateLeftOutlined => new RotateLeftOutlined(),
            AntDesignIconKind.LeftCircleOutlined => new LeftCircleOutlined(),
            AntDesignIconKind.PlayCircleOutlined => new PlayCircleOutlined(),
            AntDesignIconKind.FileZipOutlined => new FileZipOutlined(),
            AntDesignIconKind.BorderOuterOutlined => new BorderOuterOutlined(),
            AntDesignIconKind.MediumWorkmarkOutlined => new MediumWorkmarkOutlined(),
            AntDesignIconKind.DropboxOutlined => new DropboxOutlined(),
            AntDesignIconKind.InsertRowAboveOutlined => new InsertRowAboveOutlined(),
            AntDesignIconKind.HourglassOutlined => new HourglassOutlined(),
            AntDesignIconKind.SortDescendingOutlined => new SortDescendingOutlined(),
            AntDesignIconKind.SwapLeftOutlined => new SwapLeftOutlined(),
            AntDesignIconKind.SafetyOutlined => new SafetyOutlined(),
            AntDesignIconKind.MenuUnfoldOutlined => new MenuUnfoldOutlined(),
            AntDesignIconKind.NodeCollapseOutlined => new NodeCollapseOutlined(),
            AntDesignIconKind.UserOutlined => new UserOutlined(),
            AntDesignIconKind.HighlightOutlined => new HighlightOutlined(),
            AntDesignIconKind.DeliveredProcedureOutlined => new DeliveredProcedureOutlined(),
            AntDesignIconKind.FullscreenOutlined => new FullscreenOutlined(),
            _ => throw new InvalidOperationException($"Icon kind {kind} does not exist")
        };
    }

    [UnconditionalSuppressMessage("Trimming", "IL2063",
        Justification = "Every switch arm returns typeof(...) for a generated icon class with a public parameterless constructor.")]
    [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
    private static Type GetIconTypeChunk3(AntDesignIconKind kind)
    {
        switch (kind)
        {
            case AntDesignIconKind.LinuxOutlined: return typeof(LinuxOutlined);
            case AntDesignIconKind.ReconciliationOutlined: return typeof(ReconciliationOutlined);
            case AntDesignIconKind.BilibiliOutlined: return typeof(BilibiliOutlined);
            case AntDesignIconKind.IssuesCloseOutlined: return typeof(IssuesCloseOutlined);
            case AntDesignIconKind.AndroidOutlined: return typeof(AndroidOutlined);
            case AntDesignIconKind.DollarOutlined: return typeof(DollarOutlined);
            case AntDesignIconKind.WindowMinimizedOutlined: return typeof(WindowMinimizedOutlined);
            case AntDesignIconKind.PullRequestOutlined: return typeof(PullRequestOutlined);
            case AntDesignIconKind.HomeOutlined: return typeof(HomeOutlined);
            case AntDesignIconKind.PoundCircleOutlined: return typeof(PoundCircleOutlined);
            case AntDesignIconKind.ShopOutlined: return typeof(ShopOutlined);
            case AntDesignIconKind.DingtalkOutlined: return typeof(DingtalkOutlined);
            case AntDesignIconKind.CopyrightOutlined: return typeof(CopyrightOutlined);
            case AntDesignIconKind.AlertOutlined: return typeof(AlertOutlined);
            case AntDesignIconKind.GitlabOutlined: return typeof(GitlabOutlined);
            case AntDesignIconKind.DisconnectOutlined: return typeof(DisconnectOutlined);
            case AntDesignIconKind.ColumnWidthOutlined: return typeof(ColumnWidthOutlined);
            case AntDesignIconKind.SlidersOutlined: return typeof(SlidersOutlined);
            case AntDesignIconKind.DollarCircleOutlined: return typeof(DollarCircleOutlined);
            case AntDesignIconKind.AntDesignOutlined: return typeof(AntDesignOutlined);
            case AntDesignIconKind.WeiboOutlined: return typeof(WeiboOutlined);
            case AntDesignIconKind.FunctionOutlined: return typeof(FunctionOutlined);
            case AntDesignIconKind.FullscreenExitOutlined: return typeof(FullscreenExitOutlined);
            case AntDesignIconKind.LogoutOutlined: return typeof(LogoutOutlined);
            case AntDesignIconKind.JavaOutlined: return typeof(JavaOutlined);
            case AntDesignIconKind.StrikethroughOutlined: return typeof(StrikethroughOutlined);
            case AntDesignIconKind.ShoppingOutlined: return typeof(ShoppingOutlined);
            case AntDesignIconKind.FileWordOutlined: return typeof(FileWordOutlined);
            case AntDesignIconKind.FunnelPlotOutlined: return typeof(FunnelPlotOutlined);
            case AntDesignIconKind.FileGifOutlined: return typeof(FileGifOutlined);
            case AntDesignIconKind.FolderViewOutlined: return typeof(FolderViewOutlined);
            case AntDesignIconKind.GithubOutlined: return typeof(GithubOutlined);
            case AntDesignIconKind.VerticalRightOutlined: return typeof(VerticalRightOutlined);
            case AntDesignIconKind.UsbOutlined: return typeof(UsbOutlined);
            case AntDesignIconKind.EuroCircleOutlined: return typeof(EuroCircleOutlined);
            case AntDesignIconKind.ExportOutlined: return typeof(ExportOutlined);
            case AntDesignIconKind.DownOutlined: return typeof(DownOutlined);
            case AntDesignIconKind.ShrinkOutlined: return typeof(ShrinkOutlined);
            case AntDesignIconKind.UsergroupAddOutlined: return typeof(UsergroupAddOutlined);
            case AntDesignIconKind.TagOutlined: return typeof(TagOutlined);
            case AntDesignIconKind.UpSquareOutlined: return typeof(UpSquareOutlined);
            case AntDesignIconKind.OneToOneOutlined: return typeof(OneToOneOutlined);
            case AntDesignIconKind.DownSquareOutlined: return typeof(DownSquareOutlined);
            case AntDesignIconKind.HolderOutlined: return typeof(HolderOutlined);
            case AntDesignIconKind.FileAddOutlined: return typeof(FileAddOutlined);
            case AntDesignIconKind.FallOutlined: return typeof(FallOutlined);
            case AntDesignIconKind.InboxOutlined: return typeof(InboxOutlined);
            case AntDesignIconKind.AuditOutlined: return typeof(AuditOutlined);
            case AntDesignIconKind.PlusSquareOutlined: return typeof(PlusSquareOutlined);
            case AntDesignIconKind.StockOutlined: return typeof(StockOutlined);
            case AntDesignIconKind.VerifiedOutlined: return typeof(VerifiedOutlined);
            case AntDesignIconKind.DragOutlined: return typeof(DragOutlined);
            case AntDesignIconKind.DatabaseOutlined: return typeof(DatabaseOutlined);
            case AntDesignIconKind.PartitionOutlined: return typeof(PartitionOutlined);
            case AntDesignIconKind.FileOutlined: return typeof(FileOutlined);
            case AntDesignIconKind.AccountBookOutlined: return typeof(AccountBookOutlined);
            case AntDesignIconKind.ControlOutlined: return typeof(ControlOutlined);
            case AntDesignIconKind.RedEnvelopeOutlined: return typeof(RedEnvelopeOutlined);
            case AntDesignIconKind.FitToWindowOutlined: return typeof(FitToWindowOutlined);
            case AntDesignIconKind.RadiusBottomrightOutlined: return typeof(RadiusBottomrightOutlined);
            case AntDesignIconKind.ClearOutlined: return typeof(ClearOutlined);
            case AntDesignIconKind.RadiusBottomleftOutlined: return typeof(RadiusBottomleftOutlined);
            case AntDesignIconKind.BoxPlotOutlined: return typeof(BoxPlotOutlined);
            case AntDesignIconKind.FileTextOutlined: return typeof(FileTextOutlined);
            default: throw new InvalidOperationException($"Icon kind {kind} does not exist");
        }
    }

    private static Icon CreateIconChunk3(AntDesignIconKind kind)
    {
        return kind switch
        {
            AntDesignIconKind.LinuxOutlined => new LinuxOutlined(),
            AntDesignIconKind.ReconciliationOutlined => new ReconciliationOutlined(),
            AntDesignIconKind.BilibiliOutlined => new BilibiliOutlined(),
            AntDesignIconKind.IssuesCloseOutlined => new IssuesCloseOutlined(),
            AntDesignIconKind.AndroidOutlined => new AndroidOutlined(),
            AntDesignIconKind.DollarOutlined => new DollarOutlined(),
            AntDesignIconKind.WindowMinimizedOutlined => new WindowMinimizedOutlined(),
            AntDesignIconKind.PullRequestOutlined => new PullRequestOutlined(),
            AntDesignIconKind.HomeOutlined => new HomeOutlined(),
            AntDesignIconKind.PoundCircleOutlined => new PoundCircleOutlined(),
            AntDesignIconKind.ShopOutlined => new ShopOutlined(),
            AntDesignIconKind.DingtalkOutlined => new DingtalkOutlined(),
            AntDesignIconKind.CopyrightOutlined => new CopyrightOutlined(),
            AntDesignIconKind.AlertOutlined => new AlertOutlined(),
            AntDesignIconKind.GitlabOutlined => new GitlabOutlined(),
            AntDesignIconKind.DisconnectOutlined => new DisconnectOutlined(),
            AntDesignIconKind.ColumnWidthOutlined => new ColumnWidthOutlined(),
            AntDesignIconKind.SlidersOutlined => new SlidersOutlined(),
            AntDesignIconKind.DollarCircleOutlined => new DollarCircleOutlined(),
            AntDesignIconKind.AntDesignOutlined => new AntDesignOutlined(),
            AntDesignIconKind.WeiboOutlined => new WeiboOutlined(),
            AntDesignIconKind.FunctionOutlined => new FunctionOutlined(),
            AntDesignIconKind.FullscreenExitOutlined => new FullscreenExitOutlined(),
            AntDesignIconKind.LogoutOutlined => new LogoutOutlined(),
            AntDesignIconKind.JavaOutlined => new JavaOutlined(),
            AntDesignIconKind.StrikethroughOutlined => new StrikethroughOutlined(),
            AntDesignIconKind.ShoppingOutlined => new ShoppingOutlined(),
            AntDesignIconKind.FileWordOutlined => new FileWordOutlined(),
            AntDesignIconKind.FunnelPlotOutlined => new FunnelPlotOutlined(),
            AntDesignIconKind.FileGifOutlined => new FileGifOutlined(),
            AntDesignIconKind.FolderViewOutlined => new FolderViewOutlined(),
            AntDesignIconKind.GithubOutlined => new GithubOutlined(),
            AntDesignIconKind.VerticalRightOutlined => new VerticalRightOutlined(),
            AntDesignIconKind.UsbOutlined => new UsbOutlined(),
            AntDesignIconKind.EuroCircleOutlined => new EuroCircleOutlined(),
            AntDesignIconKind.ExportOutlined => new ExportOutlined(),
            AntDesignIconKind.DownOutlined => new DownOutlined(),
            AntDesignIconKind.ShrinkOutlined => new ShrinkOutlined(),
            AntDesignIconKind.UsergroupAddOutlined => new UsergroupAddOutlined(),
            AntDesignIconKind.TagOutlined => new TagOutlined(),
            AntDesignIconKind.UpSquareOutlined => new UpSquareOutlined(),
            AntDesignIconKind.OneToOneOutlined => new OneToOneOutlined(),
            AntDesignIconKind.DownSquareOutlined => new DownSquareOutlined(),
            AntDesignIconKind.HolderOutlined => new HolderOutlined(),
            AntDesignIconKind.FileAddOutlined => new FileAddOutlined(),
            AntDesignIconKind.FallOutlined => new FallOutlined(),
            AntDesignIconKind.InboxOutlined => new InboxOutlined(),
            AntDesignIconKind.AuditOutlined => new AuditOutlined(),
            AntDesignIconKind.PlusSquareOutlined => new PlusSquareOutlined(),
            AntDesignIconKind.StockOutlined => new StockOutlined(),
            AntDesignIconKind.VerifiedOutlined => new VerifiedOutlined(),
            AntDesignIconKind.DragOutlined => new DragOutlined(),
            AntDesignIconKind.DatabaseOutlined => new DatabaseOutlined(),
            AntDesignIconKind.PartitionOutlined => new PartitionOutlined(),
            AntDesignIconKind.FileOutlined => new FileOutlined(),
            AntDesignIconKind.AccountBookOutlined => new AccountBookOutlined(),
            AntDesignIconKind.ControlOutlined => new ControlOutlined(),
            AntDesignIconKind.RedEnvelopeOutlined => new RedEnvelopeOutlined(),
            AntDesignIconKind.FitToWindowOutlined => new FitToWindowOutlined(),
            AntDesignIconKind.RadiusBottomrightOutlined => new RadiusBottomrightOutlined(),
            AntDesignIconKind.ClearOutlined => new ClearOutlined(),
            AntDesignIconKind.RadiusBottomleftOutlined => new RadiusBottomleftOutlined(),
            AntDesignIconKind.BoxPlotOutlined => new BoxPlotOutlined(),
            AntDesignIconKind.FileTextOutlined => new FileTextOutlined(),
            _ => throw new InvalidOperationException($"Icon kind {kind} does not exist")
        };
    }

    [UnconditionalSuppressMessage("Trimming", "IL2063",
        Justification = "Every switch arm returns typeof(...) for a generated icon class with a public parameterless constructor.")]
    [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
    private static Type GetIconTypeChunk4(AntDesignIconKind kind)
    {
        switch (kind)
        {
            case AntDesignIconKind.FolderOpenOutlined: return typeof(FolderOpenOutlined);
            case AntDesignIconKind.BuildOutlined: return typeof(BuildOutlined);
            case AntDesignIconKind.FontSizeOutlined: return typeof(FontSizeOutlined);
            case AntDesignIconKind.XOutlined: return typeof(XOutlined);
            case AntDesignIconKind.ApartmentOutlined: return typeof(ApartmentOutlined);
            case AntDesignIconKind.BehanceSquareOutlined: return typeof(BehanceSquareOutlined);
            case AntDesignIconKind.BarChartOutlined: return typeof(BarChartOutlined);
            case AntDesignIconKind.UpOutlined: return typeof(UpOutlined);
            case AntDesignIconKind.GiteeOutlined: return typeof(GiteeOutlined);
            case AntDesignIconKind.QuestionCircleOutlined: return typeof(QuestionCircleOutlined);
            case AntDesignIconKind.QuestionOutlined: return typeof(QuestionOutlined);
            case AntDesignIconKind.LockOutlined: return typeof(LockOutlined);
            case AntDesignIconKind.CloudUploadOutlined: return typeof(CloudUploadOutlined);
            case AntDesignIconKind.FireOutlined: return typeof(FireOutlined);
            case AntDesignIconKind.WeiboCircleOutlined: return typeof(WeiboCircleOutlined);
            case AntDesignIconKind.DislikeOutlined: return typeof(DislikeOutlined);
            case AntDesignIconKind.UsergroupDeleteOutlined: return typeof(UsergroupDeleteOutlined);
            case AntDesignIconKind.HarmonyOSOutlined: return typeof(HarmonyOSOutlined);
            case AntDesignIconKind.InstagramOutlined: return typeof(InstagramOutlined);
            case AntDesignIconKind.EuroOutlined: return typeof(EuroOutlined);
            case AntDesignIconKind.LinkOutlined: return typeof(LinkOutlined);
            case AntDesignIconKind.IdcardOutlined: return typeof(IdcardOutlined);
            case AntDesignIconKind.ProductOutlined: return typeof(ProductOutlined);
            case AntDesignIconKind.KeyOutlined: return typeof(KeyOutlined);
            case AntDesignIconKind.NodeIndexOutlined: return typeof(NodeIndexOutlined);
            case AntDesignIconKind.MehOutlined: return typeof(MehOutlined);
            case AntDesignIconKind.WhatsAppOutlined: return typeof(WhatsAppOutlined);
            case AntDesignIconKind.ArrowRightOutlined: return typeof(ArrowRightOutlined);
            case AntDesignIconKind.PercentageOutlined: return typeof(PercentageOutlined);
            case AntDesignIconKind.FieldNumberOutlined: return typeof(FieldNumberOutlined);
            case AntDesignIconKind.RubyOutlined: return typeof(RubyOutlined);
            case AntDesignIconKind.QrcodeOutlined: return typeof(QrcodeOutlined);
            case AntDesignIconKind.NumberOutlined: return typeof(NumberOutlined);
            case AntDesignIconKind.CiOutlined: return typeof(CiOutlined);
            case AntDesignIconKind.DiffOutlined: return typeof(DiffOutlined);
            case AntDesignIconKind.GroupOutlined: return typeof(GroupOutlined);
            case AntDesignIconKind.MinusSquareOutlined: return typeof(MinusSquareOutlined);
            case AntDesignIconKind.CompressOutlined: return typeof(CompressOutlined);
            case AntDesignIconKind.BorderInnerOutlined: return typeof(BorderInnerOutlined);
            case AntDesignIconKind.PaperClipOutlined: return typeof(PaperClipOutlined);
            case AntDesignIconKind.PythonOutlined: return typeof(PythonOutlined);
            case AntDesignIconKind.CaretUpOutlined: return typeof(CaretUpOutlined);
            case AntDesignIconKind.CloseCircleOutlined: return typeof(CloseCircleOutlined);
            case AntDesignIconKind.PicCenterOutlined: return typeof(PicCenterOutlined);
            case AntDesignIconKind.MailOutlined: return typeof(MailOutlined);
            case AntDesignIconKind.DesktopOutlined: return typeof(DesktopOutlined);
            case AntDesignIconKind.DownloadOutlined: return typeof(DownloadOutlined);
            case AntDesignIconKind.ExpandOutlined: return typeof(ExpandOutlined);
            case AntDesignIconKind.DockerOutlined: return typeof(DockerOutlined);
            case AntDesignIconKind.ImportOutlined: return typeof(ImportOutlined);
            case AntDesignIconKind.KubernetesOutlined: return typeof(KubernetesOutlined);
            case AntDesignIconKind.RadarChartOutlined: return typeof(RadarChartOutlined);
            case AntDesignIconKind.AreaChartOutlined: return typeof(AreaChartOutlined);
            case AntDesignIconKind.PoundOutlined: return typeof(PoundOutlined);
            case AntDesignIconKind.InsertRowLeftOutlined: return typeof(InsertRowLeftOutlined);
            case AntDesignIconKind.FileDoneOutlined: return typeof(FileDoneOutlined);
            case AntDesignIconKind.UserDeleteOutlined: return typeof(UserDeleteOutlined);
            case AntDesignIconKind.ConsoleSqlOutlined: return typeof(ConsoleSqlOutlined);
            case AntDesignIconKind.AliyunOutlined: return typeof(AliyunOutlined);
            case AntDesignIconKind.LoginOutlined: return typeof(LoginOutlined);
            case AntDesignIconKind.BookOutlined: return typeof(BookOutlined);
            case AntDesignIconKind.ScissorOutlined: return typeof(ScissorOutlined);
            case AntDesignIconKind.WalletOutlined: return typeof(WalletOutlined);
            case AntDesignIconKind.FileImageOutlined: return typeof(FileImageOutlined);
            default: throw new InvalidOperationException($"Icon kind {kind} does not exist");
        }
    }

    private static Icon CreateIconChunk4(AntDesignIconKind kind)
    {
        return kind switch
        {
            AntDesignIconKind.FolderOpenOutlined => new FolderOpenOutlined(),
            AntDesignIconKind.BuildOutlined => new BuildOutlined(),
            AntDesignIconKind.FontSizeOutlined => new FontSizeOutlined(),
            AntDesignIconKind.XOutlined => new XOutlined(),
            AntDesignIconKind.ApartmentOutlined => new ApartmentOutlined(),
            AntDesignIconKind.BehanceSquareOutlined => new BehanceSquareOutlined(),
            AntDesignIconKind.BarChartOutlined => new BarChartOutlined(),
            AntDesignIconKind.UpOutlined => new UpOutlined(),
            AntDesignIconKind.GiteeOutlined => new GiteeOutlined(),
            AntDesignIconKind.QuestionCircleOutlined => new QuestionCircleOutlined(),
            AntDesignIconKind.QuestionOutlined => new QuestionOutlined(),
            AntDesignIconKind.LockOutlined => new LockOutlined(),
            AntDesignIconKind.CloudUploadOutlined => new CloudUploadOutlined(),
            AntDesignIconKind.FireOutlined => new FireOutlined(),
            AntDesignIconKind.WeiboCircleOutlined => new WeiboCircleOutlined(),
            AntDesignIconKind.DislikeOutlined => new DislikeOutlined(),
            AntDesignIconKind.UsergroupDeleteOutlined => new UsergroupDeleteOutlined(),
            AntDesignIconKind.HarmonyOSOutlined => new HarmonyOSOutlined(),
            AntDesignIconKind.InstagramOutlined => new InstagramOutlined(),
            AntDesignIconKind.EuroOutlined => new EuroOutlined(),
            AntDesignIconKind.LinkOutlined => new LinkOutlined(),
            AntDesignIconKind.IdcardOutlined => new IdcardOutlined(),
            AntDesignIconKind.ProductOutlined => new ProductOutlined(),
            AntDesignIconKind.KeyOutlined => new KeyOutlined(),
            AntDesignIconKind.NodeIndexOutlined => new NodeIndexOutlined(),
            AntDesignIconKind.MehOutlined => new MehOutlined(),
            AntDesignIconKind.WhatsAppOutlined => new WhatsAppOutlined(),
            AntDesignIconKind.ArrowRightOutlined => new ArrowRightOutlined(),
            AntDesignIconKind.PercentageOutlined => new PercentageOutlined(),
            AntDesignIconKind.FieldNumberOutlined => new FieldNumberOutlined(),
            AntDesignIconKind.RubyOutlined => new RubyOutlined(),
            AntDesignIconKind.QrcodeOutlined => new QrcodeOutlined(),
            AntDesignIconKind.NumberOutlined => new NumberOutlined(),
            AntDesignIconKind.CiOutlined => new CiOutlined(),
            AntDesignIconKind.DiffOutlined => new DiffOutlined(),
            AntDesignIconKind.GroupOutlined => new GroupOutlined(),
            AntDesignIconKind.MinusSquareOutlined => new MinusSquareOutlined(),
            AntDesignIconKind.CompressOutlined => new CompressOutlined(),
            AntDesignIconKind.BorderInnerOutlined => new BorderInnerOutlined(),
            AntDesignIconKind.PaperClipOutlined => new PaperClipOutlined(),
            AntDesignIconKind.PythonOutlined => new PythonOutlined(),
            AntDesignIconKind.CaretUpOutlined => new CaretUpOutlined(),
            AntDesignIconKind.CloseCircleOutlined => new CloseCircleOutlined(),
            AntDesignIconKind.PicCenterOutlined => new PicCenterOutlined(),
            AntDesignIconKind.MailOutlined => new MailOutlined(),
            AntDesignIconKind.DesktopOutlined => new DesktopOutlined(),
            AntDesignIconKind.DownloadOutlined => new DownloadOutlined(),
            AntDesignIconKind.ExpandOutlined => new ExpandOutlined(),
            AntDesignIconKind.DockerOutlined => new DockerOutlined(),
            AntDesignIconKind.ImportOutlined => new ImportOutlined(),
            AntDesignIconKind.KubernetesOutlined => new KubernetesOutlined(),
            AntDesignIconKind.RadarChartOutlined => new RadarChartOutlined(),
            AntDesignIconKind.AreaChartOutlined => new AreaChartOutlined(),
            AntDesignIconKind.PoundOutlined => new PoundOutlined(),
            AntDesignIconKind.InsertRowLeftOutlined => new InsertRowLeftOutlined(),
            AntDesignIconKind.FileDoneOutlined => new FileDoneOutlined(),
            AntDesignIconKind.UserDeleteOutlined => new UserDeleteOutlined(),
            AntDesignIconKind.ConsoleSqlOutlined => new ConsoleSqlOutlined(),
            AntDesignIconKind.AliyunOutlined => new AliyunOutlined(),
            AntDesignIconKind.LoginOutlined => new LoginOutlined(),
            AntDesignIconKind.BookOutlined => new BookOutlined(),
            AntDesignIconKind.ScissorOutlined => new ScissorOutlined(),
            AntDesignIconKind.WalletOutlined => new WalletOutlined(),
            AntDesignIconKind.FileImageOutlined => new FileImageOutlined(),
            _ => throw new InvalidOperationException($"Icon kind {kind} does not exist")
        };
    }

    [UnconditionalSuppressMessage("Trimming", "IL2063",
        Justification = "Every switch arm returns typeof(...) for a generated icon class with a public parameterless constructor.")]
    [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
    private static Type GetIconTypeChunk5(AntDesignIconKind kind)
    {
        switch (kind)
        {
            case AntDesignIconKind.BellOutlined: return typeof(BellOutlined);
            case AntDesignIconKind.CoffeeOutlined: return typeof(CoffeeOutlined);
            case AntDesignIconKind.DashboardOutlined: return typeof(DashboardOutlined);
            case AntDesignIconKind.CodeOutlined: return typeof(CodeOutlined);
            case AntDesignIconKind.PoweroffOutlined: return typeof(PoweroffOutlined);
            case AntDesignIconKind.HeatMapOutlined: return typeof(HeatMapOutlined);
            case AntDesignIconKind.AlipayCircleOutlined: return typeof(AlipayCircleOutlined);
            case AntDesignIconKind.AliwangwangOutlined: return typeof(AliwangwangOutlined);
            case AntDesignIconKind.CarryOutOutlined: return typeof(CarryOutOutlined);
            case AntDesignIconKind.GifOutlined: return typeof(GifOutlined);
            case AntDesignIconKind.TransactionOutlined: return typeof(TransactionOutlined);
            case AntDesignIconKind.DoubleLeftOutlined: return typeof(DoubleLeftOutlined);
            case AntDesignIconKind.FlagOutlined: return typeof(FlagOutlined);
            case AntDesignIconKind.SnippetsOutlined: return typeof(SnippetsOutlined);
            case AntDesignIconKind.StopOutlined: return typeof(StopOutlined);
            case AntDesignIconKind.RightCircleOutlined: return typeof(RightCircleOutlined);
            case AntDesignIconKind.StepForwardOutlined: return typeof(StepForwardOutlined);
            case AntDesignIconKind.ClusterOutlined: return typeof(ClusterOutlined);
            case AntDesignIconKind.MutedOutlined: return typeof(MutedOutlined);
            case AntDesignIconKind.ContainerOutlined: return typeof(ContainerOutlined);
            case AntDesignIconKind.SlackSquareOutlined: return typeof(SlackSquareOutlined);
            case AntDesignIconKind.FrownOutlined: return typeof(FrownOutlined);
            case AntDesignIconKind.ToolOutlined: return typeof(ToolOutlined);
            case AntDesignIconKind.SafetyCertificateOutlined: return typeof(SafetyCertificateOutlined);
            case AntDesignIconKind.DribbbleSquareOutlined: return typeof(DribbbleSquareOutlined);
            case AntDesignIconKind.RiseOutlined: return typeof(RiseOutlined);
            case AntDesignIconKind.FileSyncOutlined: return typeof(FileSyncOutlined);
            case AntDesignIconKind.BorderOutlined: return typeof(BorderOutlined);
            case AntDesignIconKind.CaretRightOutlined: return typeof(CaretRightOutlined);
            case AntDesignIconKind.BoldOutlined: return typeof(BoldOutlined);
            case AntDesignIconKind.TrophyOutlined: return typeof(TrophyOutlined);
            case AntDesignIconKind.ShareAltOutlined: return typeof(ShareAltOutlined);
            case AntDesignIconKind.PlusOutlined: return typeof(PlusOutlined);
            case AntDesignIconKind.CheckOutlined: return typeof(CheckOutlined);
            case AntDesignIconKind.DashOutlined: return typeof(DashOutlined);
            case AntDesignIconKind.UserSwitchOutlined: return typeof(UserSwitchOutlined);
            case AntDesignIconKind.WarningOutlined: return typeof(WarningOutlined);
            case AntDesignIconKind.TrademarkOutlined: return typeof(TrademarkOutlined);
            case AntDesignIconKind.GlobalOutlined: return typeof(GlobalOutlined);
            case AntDesignIconKind.AudioMutedOutlined: return typeof(AudioMutedOutlined);
            case AntDesignIconKind.PieChartOutlined: return typeof(PieChartOutlined);
            case AntDesignIconKind.BorderlessTableOutlined: return typeof(BorderlessTableOutlined);
            case AntDesignIconKind.RadiusUprightOutlined: return typeof(RadiusUprightOutlined);
            case AntDesignIconKind.SecurityScanOutlined: return typeof(SecurityScanOutlined);
            case AntDesignIconKind.NodeExpandOutlined: return typeof(NodeExpandOutlined);
            case AntDesignIconKind.WifiOutlined: return typeof(WifiOutlined);
            case AntDesignIconKind.LineHeightOutlined: return typeof(LineHeightOutlined);
            case AntDesignIconKind.TranslationOutlined: return typeof(TranslationOutlined);
            case AntDesignIconKind.TeamOutlined: return typeof(TeamOutlined);
            case AntDesignIconKind.InfoOutlined: return typeof(InfoOutlined);
            case AntDesignIconKind.SubnodeOutlined: return typeof(SubnodeOutlined);
            case AntDesignIconKind.SpotifyOutlined: return typeof(SpotifyOutlined);
            case AntDesignIconKind.JavaScriptOutlined: return typeof(JavaScriptOutlined);
            case AntDesignIconKind.UserAddOutlined: return typeof(UserAddOutlined);
            case AntDesignIconKind.CloseOutlined: return typeof(CloseOutlined);
            case AntDesignIconKind.BorderVerticleOutlined: return typeof(BorderVerticleOutlined);
            case AntDesignIconKind.InfoCircleOutlined: return typeof(InfoCircleOutlined);
            case AntDesignIconKind.DingdingOutlined: return typeof(DingdingOutlined);
            case AntDesignIconKind.EyeInvisibleOutlined: return typeof(EyeInvisibleOutlined);
            case AntDesignIconKind.CloudSyncOutlined: return typeof(CloudSyncOutlined);
            case AntDesignIconKind.LeftSquareOutlined: return typeof(LeftSquareOutlined);
            case AntDesignIconKind.CopyOutlined: return typeof(CopyOutlined);
            case AntDesignIconKind.GoldOutlined: return typeof(GoldOutlined);
            case AntDesignIconKind.AntCloudOutlined: return typeof(AntCloudOutlined);
            default: throw new InvalidOperationException($"Icon kind {kind} does not exist");
        }
    }

    private static Icon CreateIconChunk5(AntDesignIconKind kind)
    {
        return kind switch
        {
            AntDesignIconKind.BellOutlined => new BellOutlined(),
            AntDesignIconKind.CoffeeOutlined => new CoffeeOutlined(),
            AntDesignIconKind.DashboardOutlined => new DashboardOutlined(),
            AntDesignIconKind.CodeOutlined => new CodeOutlined(),
            AntDesignIconKind.PoweroffOutlined => new PoweroffOutlined(),
            AntDesignIconKind.HeatMapOutlined => new HeatMapOutlined(),
            AntDesignIconKind.AlipayCircleOutlined => new AlipayCircleOutlined(),
            AntDesignIconKind.AliwangwangOutlined => new AliwangwangOutlined(),
            AntDesignIconKind.CarryOutOutlined => new CarryOutOutlined(),
            AntDesignIconKind.GifOutlined => new GifOutlined(),
            AntDesignIconKind.TransactionOutlined => new TransactionOutlined(),
            AntDesignIconKind.DoubleLeftOutlined => new DoubleLeftOutlined(),
            AntDesignIconKind.FlagOutlined => new FlagOutlined(),
            AntDesignIconKind.SnippetsOutlined => new SnippetsOutlined(),
            AntDesignIconKind.StopOutlined => new StopOutlined(),
            AntDesignIconKind.RightCircleOutlined => new RightCircleOutlined(),
            AntDesignIconKind.StepForwardOutlined => new StepForwardOutlined(),
            AntDesignIconKind.ClusterOutlined => new ClusterOutlined(),
            AntDesignIconKind.MutedOutlined => new MutedOutlined(),
            AntDesignIconKind.ContainerOutlined => new ContainerOutlined(),
            AntDesignIconKind.SlackSquareOutlined => new SlackSquareOutlined(),
            AntDesignIconKind.FrownOutlined => new FrownOutlined(),
            AntDesignIconKind.ToolOutlined => new ToolOutlined(),
            AntDesignIconKind.SafetyCertificateOutlined => new SafetyCertificateOutlined(),
            AntDesignIconKind.DribbbleSquareOutlined => new DribbbleSquareOutlined(),
            AntDesignIconKind.RiseOutlined => new RiseOutlined(),
            AntDesignIconKind.FileSyncOutlined => new FileSyncOutlined(),
            AntDesignIconKind.BorderOutlined => new BorderOutlined(),
            AntDesignIconKind.CaretRightOutlined => new CaretRightOutlined(),
            AntDesignIconKind.BoldOutlined => new BoldOutlined(),
            AntDesignIconKind.TrophyOutlined => new TrophyOutlined(),
            AntDesignIconKind.ShareAltOutlined => new ShareAltOutlined(),
            AntDesignIconKind.PlusOutlined => new PlusOutlined(),
            AntDesignIconKind.CheckOutlined => new CheckOutlined(),
            AntDesignIconKind.DashOutlined => new DashOutlined(),
            AntDesignIconKind.UserSwitchOutlined => new UserSwitchOutlined(),
            AntDesignIconKind.WarningOutlined => new WarningOutlined(),
            AntDesignIconKind.TrademarkOutlined => new TrademarkOutlined(),
            AntDesignIconKind.GlobalOutlined => new GlobalOutlined(),
            AntDesignIconKind.AudioMutedOutlined => new AudioMutedOutlined(),
            AntDesignIconKind.PieChartOutlined => new PieChartOutlined(),
            AntDesignIconKind.BorderlessTableOutlined => new BorderlessTableOutlined(),
            AntDesignIconKind.RadiusUprightOutlined => new RadiusUprightOutlined(),
            AntDesignIconKind.SecurityScanOutlined => new SecurityScanOutlined(),
            AntDesignIconKind.NodeExpandOutlined => new NodeExpandOutlined(),
            AntDesignIconKind.WifiOutlined => new WifiOutlined(),
            AntDesignIconKind.LineHeightOutlined => new LineHeightOutlined(),
            AntDesignIconKind.TranslationOutlined => new TranslationOutlined(),
            AntDesignIconKind.TeamOutlined => new TeamOutlined(),
            AntDesignIconKind.InfoOutlined => new InfoOutlined(),
            AntDesignIconKind.SubnodeOutlined => new SubnodeOutlined(),
            AntDesignIconKind.SpotifyOutlined => new SpotifyOutlined(),
            AntDesignIconKind.JavaScriptOutlined => new JavaScriptOutlined(),
            AntDesignIconKind.UserAddOutlined => new UserAddOutlined(),
            AntDesignIconKind.CloseOutlined => new CloseOutlined(),
            AntDesignIconKind.BorderVerticleOutlined => new BorderVerticleOutlined(),
            AntDesignIconKind.InfoCircleOutlined => new InfoCircleOutlined(),
            AntDesignIconKind.DingdingOutlined => new DingdingOutlined(),
            AntDesignIconKind.EyeInvisibleOutlined => new EyeInvisibleOutlined(),
            AntDesignIconKind.CloudSyncOutlined => new CloudSyncOutlined(),
            AntDesignIconKind.LeftSquareOutlined => new LeftSquareOutlined(),
            AntDesignIconKind.CopyOutlined => new CopyOutlined(),
            AntDesignIconKind.GoldOutlined => new GoldOutlined(),
            AntDesignIconKind.AntCloudOutlined => new AntCloudOutlined(),
            _ => throw new InvalidOperationException($"Icon kind {kind} does not exist")
        };
    }

    [UnconditionalSuppressMessage("Trimming", "IL2063",
        Justification = "Every switch arm returns typeof(...) for a generated icon class with a public parameterless constructor.")]
    [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
    private static Type GetIconTypeChunk6(AntDesignIconKind kind)
    {
        switch (kind)
        {
            case AntDesignIconKind.ZoomInOutlined: return typeof(ZoomInOutlined);
            case AntDesignIconKind.FundOutlined: return typeof(FundOutlined);
            case AntDesignIconKind.SignatureOutlined: return typeof(SignatureOutlined);
            case AntDesignIconKind.ReloadOutlined: return typeof(ReloadOutlined);
            case AntDesignIconKind.PlaySquareOutlined: return typeof(PlaySquareOutlined);
            case AntDesignIconKind.BorderRightOutlined: return typeof(BorderRightOutlined);
            case AntDesignIconKind.FileExclamationOutlined: return typeof(FileExclamationOutlined);
            case AntDesignIconKind.CodepenCircleOutlined: return typeof(CodepenCircleOutlined);
            case AntDesignIconKind.FastBackwardOutlined: return typeof(FastBackwardOutlined);
            case AntDesignIconKind.AlignRightOutlined: return typeof(AlignRightOutlined);
            case AntDesignIconKind.EnvironmentOutlined: return typeof(EnvironmentOutlined);
            case AntDesignIconKind.CheckCircleOutlined: return typeof(CheckCircleOutlined);
            case AntDesignIconKind.Html5Outlined: return typeof(Html5Outlined);
            case AntDesignIconKind.SaveOutlined: return typeof(SaveOutlined);
            case AntDesignIconKind.SmileOutlined: return typeof(SmileOutlined);
            case AntDesignIconKind.ScanOutlined: return typeof(ScanOutlined);
            case AntDesignIconKind.ForkOutlined: return typeof(ForkOutlined);
            case AntDesignIconKind.RightOutlined: return typeof(RightOutlined);
            case AntDesignIconKind.FileProtectOutlined: return typeof(FileProtectOutlined);
            case AntDesignIconKind.AimOutlined: return typeof(AimOutlined);
            case AntDesignIconKind.SettingOutlined: return typeof(SettingOutlined);
            case AntDesignIconKind.SolutionOutlined: return typeof(SolutionOutlined);
            case AntDesignIconKind.LoadingOutlined: return typeof(LoadingOutlined);
            case AntDesignIconKind.FastForwardOutlined: return typeof(FastForwardOutlined);
            case AntDesignIconKind.MessageOutlined: return typeof(MessageOutlined);
            case AntDesignIconKind.ZoomOutOutlined: return typeof(ZoomOutOutlined);
            case AntDesignIconKind.CopyrightCircleOutlined: return typeof(CopyrightCircleOutlined);
            case AntDesignIconKind.CrownOutlined: return typeof(CrownOutlined);
            case AntDesignIconKind.BackwardOutlined: return typeof(BackwardOutlined);
            case AntDesignIconKind.YuqueOutlined: return typeof(YuqueOutlined);
            case AntDesignIconKind.NotificationOutlined: return typeof(NotificationOutlined);
            case AntDesignIconKind.PictureOutlined: return typeof(PictureOutlined);
            case AntDesignIconKind.ExceptionOutlined: return typeof(ExceptionOutlined);
            case AntDesignIconKind.TableOutlined: return typeof(TableOutlined);
            case AntDesignIconKind.SendOutlined: return typeof(SendOutlined);
            case AntDesignIconKind.BarcodeOutlined: return typeof(BarcodeOutlined);
            case AntDesignIconKind.TaobaoCircleOutlined: return typeof(TaobaoCircleOutlined);
            case AntDesignIconKind.ColumnHeightOutlined: return typeof(ColumnHeightOutlined);
            case AntDesignIconKind.FacebookOutlined: return typeof(FacebookOutlined);
            case AntDesignIconKind.CameraOutlined: return typeof(CameraOutlined);
            case AntDesignIconKind.PrinterOutlined: return typeof(PrinterOutlined);
            case AntDesignIconKind.AmazonOutlined: return typeof(AmazonOutlined);
            case AntDesignIconKind.FileJpgOutlined: return typeof(FileJpgOutlined);
            case AntDesignIconKind.BorderTopOutlined: return typeof(BorderTopOutlined);
            case AntDesignIconKind.RedoOutlined: return typeof(RedoOutlined);
            case AntDesignIconKind.PinterestOutlined: return typeof(PinterestOutlined);
            case AntDesignIconKind.TruckOutlined: return typeof(TruckOutlined);
            case AntDesignIconKind.PayCircleOutlined: return typeof(PayCircleOutlined);
            case AntDesignIconKind.WechatOutlined: return typeof(WechatOutlined);
            case AntDesignIconKind.SwapRightOutlined: return typeof(SwapRightOutlined);
            case AntDesignIconKind.OpenAIOutlined: return typeof(OpenAIOutlined);
            case AntDesignIconKind.GoogleOutlined: return typeof(GoogleOutlined);
            case AntDesignIconKind.FormatPainterOutlined: return typeof(FormatPainterOutlined);
            case AntDesignIconKind.MoreOutlined: return typeof(MoreOutlined);
            case AntDesignIconKind.LineOutlined: return typeof(LineOutlined);
            case AntDesignIconKind.UpCircleOutlined: return typeof(UpCircleOutlined);
            case AntDesignIconKind.ExclamationCircleOutlined: return typeof(ExclamationCircleOutlined);
            case AntDesignIconKind.BranchesOutlined: return typeof(BranchesOutlined);
            case AntDesignIconKind.RadiusSettingOutlined: return typeof(RadiusSettingOutlined);
            case AntDesignIconKind.DeploymentUnitOutlined: return typeof(DeploymentUnitOutlined);
            case AntDesignIconKind.MacCommandOutlined: return typeof(MacCommandOutlined);
            case AntDesignIconKind.DownCircleOutlined: return typeof(DownCircleOutlined);
            case AntDesignIconKind.UngroupOutlined: return typeof(UngroupOutlined);
            case AntDesignIconKind.StepBackwardOutlined: return typeof(StepBackwardOutlined);
            default: throw new InvalidOperationException($"Icon kind {kind} does not exist");
        }
    }

    private static Icon CreateIconChunk6(AntDesignIconKind kind)
    {
        return kind switch
        {
            AntDesignIconKind.ZoomInOutlined => new ZoomInOutlined(),
            AntDesignIconKind.FundOutlined => new FundOutlined(),
            AntDesignIconKind.SignatureOutlined => new SignatureOutlined(),
            AntDesignIconKind.ReloadOutlined => new ReloadOutlined(),
            AntDesignIconKind.PlaySquareOutlined => new PlaySquareOutlined(),
            AntDesignIconKind.BorderRightOutlined => new BorderRightOutlined(),
            AntDesignIconKind.FileExclamationOutlined => new FileExclamationOutlined(),
            AntDesignIconKind.CodepenCircleOutlined => new CodepenCircleOutlined(),
            AntDesignIconKind.FastBackwardOutlined => new FastBackwardOutlined(),
            AntDesignIconKind.AlignRightOutlined => new AlignRightOutlined(),
            AntDesignIconKind.EnvironmentOutlined => new EnvironmentOutlined(),
            AntDesignIconKind.CheckCircleOutlined => new CheckCircleOutlined(),
            AntDesignIconKind.Html5Outlined => new Html5Outlined(),
            AntDesignIconKind.SaveOutlined => new SaveOutlined(),
            AntDesignIconKind.SmileOutlined => new SmileOutlined(),
            AntDesignIconKind.ScanOutlined => new ScanOutlined(),
            AntDesignIconKind.ForkOutlined => new ForkOutlined(),
            AntDesignIconKind.RightOutlined => new RightOutlined(),
            AntDesignIconKind.FileProtectOutlined => new FileProtectOutlined(),
            AntDesignIconKind.AimOutlined => new AimOutlined(),
            AntDesignIconKind.SettingOutlined => new SettingOutlined(),
            AntDesignIconKind.SolutionOutlined => new SolutionOutlined(),
            AntDesignIconKind.LoadingOutlined => new LoadingOutlined(),
            AntDesignIconKind.FastForwardOutlined => new FastForwardOutlined(),
            AntDesignIconKind.MessageOutlined => new MessageOutlined(),
            AntDesignIconKind.ZoomOutOutlined => new ZoomOutOutlined(),
            AntDesignIconKind.CopyrightCircleOutlined => new CopyrightCircleOutlined(),
            AntDesignIconKind.CrownOutlined => new CrownOutlined(),
            AntDesignIconKind.BackwardOutlined => new BackwardOutlined(),
            AntDesignIconKind.YuqueOutlined => new YuqueOutlined(),
            AntDesignIconKind.NotificationOutlined => new NotificationOutlined(),
            AntDesignIconKind.PictureOutlined => new PictureOutlined(),
            AntDesignIconKind.ExceptionOutlined => new ExceptionOutlined(),
            AntDesignIconKind.TableOutlined => new TableOutlined(),
            AntDesignIconKind.SendOutlined => new SendOutlined(),
            AntDesignIconKind.BarcodeOutlined => new BarcodeOutlined(),
            AntDesignIconKind.TaobaoCircleOutlined => new TaobaoCircleOutlined(),
            AntDesignIconKind.ColumnHeightOutlined => new ColumnHeightOutlined(),
            AntDesignIconKind.FacebookOutlined => new FacebookOutlined(),
            AntDesignIconKind.CameraOutlined => new CameraOutlined(),
            AntDesignIconKind.PrinterOutlined => new PrinterOutlined(),
            AntDesignIconKind.AmazonOutlined => new AmazonOutlined(),
            AntDesignIconKind.FileJpgOutlined => new FileJpgOutlined(),
            AntDesignIconKind.BorderTopOutlined => new BorderTopOutlined(),
            AntDesignIconKind.RedoOutlined => new RedoOutlined(),
            AntDesignIconKind.PinterestOutlined => new PinterestOutlined(),
            AntDesignIconKind.TruckOutlined => new TruckOutlined(),
            AntDesignIconKind.PayCircleOutlined => new PayCircleOutlined(),
            AntDesignIconKind.WechatOutlined => new WechatOutlined(),
            AntDesignIconKind.SwapRightOutlined => new SwapRightOutlined(),
            AntDesignIconKind.OpenAIOutlined => new OpenAIOutlined(),
            AntDesignIconKind.GoogleOutlined => new GoogleOutlined(),
            AntDesignIconKind.FormatPainterOutlined => new FormatPainterOutlined(),
            AntDesignIconKind.MoreOutlined => new MoreOutlined(),
            AntDesignIconKind.LineOutlined => new LineOutlined(),
            AntDesignIconKind.UpCircleOutlined => new UpCircleOutlined(),
            AntDesignIconKind.ExclamationCircleOutlined => new ExclamationCircleOutlined(),
            AntDesignIconKind.BranchesOutlined => new BranchesOutlined(),
            AntDesignIconKind.RadiusSettingOutlined => new RadiusSettingOutlined(),
            AntDesignIconKind.DeploymentUnitOutlined => new DeploymentUnitOutlined(),
            AntDesignIconKind.MacCommandOutlined => new MacCommandOutlined(),
            AntDesignIconKind.DownCircleOutlined => new DownCircleOutlined(),
            AntDesignIconKind.UngroupOutlined => new UngroupOutlined(),
            AntDesignIconKind.StepBackwardOutlined => new StepBackwardOutlined(),
            _ => throw new InvalidOperationException($"Icon kind {kind} does not exist")
        };
    }

    [UnconditionalSuppressMessage("Trimming", "IL2063",
        Justification = "Every switch arm returns typeof(...) for a generated icon class with a public parameterless constructor.")]
    [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
    private static Type GetIconTypeChunk7(AntDesignIconKind kind)
    {
        switch (kind)
        {
            case AntDesignIconKind.FundViewOutlined: return typeof(FundViewOutlined);
            case AntDesignIconKind.RestOutlined: return typeof(RestOutlined);
            case AntDesignIconKind.ContactsOutlined: return typeof(ContactsOutlined);
            case AntDesignIconKind.FormOutlined: return typeof(FormOutlined);
            case AntDesignIconKind.StarOutlined: return typeof(StarOutlined);
            case AntDesignIconKind.TrademarkCircleOutlined: return typeof(TrademarkCircleOutlined);
            case AntDesignIconKind.WindowUnpinOutlined: return typeof(WindowUnpinOutlined);
            case AntDesignIconKind.SunOutlined: return typeof(SunOutlined);
            case AntDesignIconKind.OrderedListOutlined: return typeof(OrderedListOutlined);
            case AntDesignIconKind.ExperimentOutlined: return typeof(ExperimentOutlined);
            case AntDesignIconKind.DeleteColumnOutlined: return typeof(DeleteColumnOutlined);
            case AntDesignIconKind.EditOutlined: return typeof(EditOutlined);
            case AntDesignIconKind.ApiOutlined: return typeof(ApiOutlined);
            case AntDesignIconKind.TwitchOutlined: return typeof(TwitchOutlined);
            case AntDesignIconKind.WindowsOutlined: return typeof(WindowsOutlined);
            case AntDesignIconKind.BugOutlined: return typeof(BugOutlined);
            case AntDesignIconKind.YoutubeOutlined: return typeof(YoutubeOutlined);
            case AntDesignIconKind.UnlockOutlined: return typeof(UnlockOutlined);
            case AntDesignIconKind.WindowPinOutlined: return typeof(WindowPinOutlined);
            case AntDesignIconKind.ToTopOutlined: return typeof(ToTopOutlined);
            case AntDesignIconKind.BorderLeftOutlined: return typeof(BorderLeftOutlined);
            case AntDesignIconKind.CompassOutlined: return typeof(CompassOutlined);
            case AntDesignIconKind.AlibabaOutlined: return typeof(AlibabaOutlined);
            case AntDesignIconKind.PlusCircleOutlined: return typeof(PlusCircleOutlined);
            case AntDesignIconKind.MenuFoldOutlined: return typeof(MenuFoldOutlined);
            case AntDesignIconKind.SisternodeOutlined: return typeof(SisternodeOutlined);
            case AntDesignIconKind.WomanOutlined: return typeof(WomanOutlined);
            case AntDesignIconKind.BankOutlined: return typeof(BankOutlined);
            case AntDesignIconKind.CreditCardOutlined: return typeof(CreditCardOutlined);
            case AntDesignIconKind.FileMarkdownOutlined: return typeof(FileMarkdownOutlined);
            case AntDesignIconKind.VerticalAlignBottomOutlined: return typeof(VerticalAlignBottomOutlined);
            case AntDesignIconKind.GatewayOutlined: return typeof(GatewayOutlined);
            case AntDesignIconKind.SelectOutlined: return typeof(SelectOutlined);
            case AntDesignIconKind.YahooOutlined: return typeof(YahooOutlined);
            case AntDesignIconKind.Loading3QuartersOutlined: return typeof(Loading3QuartersOutlined);
            case AntDesignIconKind.WindowCloseOutlined: return typeof(WindowCloseOutlined);
            case AntDesignIconKind.SketchOutlined: return typeof(SketchOutlined);
            case AntDesignIconKind.AudioOutlined: return typeof(AudioOutlined);
            case AntDesignIconKind.DeleteOutlined: return typeof(DeleteOutlined);
            case AntDesignIconKind.RedditOutlined: return typeof(RedditOutlined);
            case AntDesignIconKind.AlipayOutlined: return typeof(AlipayOutlined);
            case AntDesignIconKind.PicRightOutlined: return typeof(PicRightOutlined);
            case AntDesignIconKind.RetweetOutlined: return typeof(RetweetOutlined);
            case AntDesignIconKind.SkinOutlined: return typeof(SkinOutlined);
            case AntDesignIconKind.PhoneOutlined: return typeof(PhoneOutlined);
            case AntDesignIconKind.WechatWorkOutlined: return typeof(WechatWorkOutlined);
            case AntDesignIconKind.SyncOutlined: return typeof(SyncOutlined);
            case AntDesignIconKind.EyeOutlined: return typeof(EyeOutlined);
            case AntDesignIconKind.MobileOutlined: return typeof(MobileOutlined);
            case AntDesignIconKind.InsuranceOutlined: return typeof(InsuranceOutlined);
            case AntDesignIconKind.CodepenOutlined: return typeof(CodepenOutlined);
            case AntDesignIconKind.DribbbleOutlined: return typeof(DribbbleOutlined);
            case AntDesignIconKind.GiftOutlined: return typeof(GiftOutlined);
            case AntDesignIconKind.LineChartOutlined: return typeof(LineChartOutlined);
            case AntDesignIconKind.CarOutlined: return typeof(CarOutlined);
            case AntDesignIconKind.CiCircleOutlined: return typeof(CiCircleOutlined);
            case AntDesignIconKind.WeiboSquareOutlined: return typeof(WeiboSquareOutlined);
            case AntDesignIconKind.ThunderboltOutlined: return typeof(ThunderboltOutlined);
            case AntDesignIconKind.ProfileOutlined: return typeof(ProfileOutlined);
            case AntDesignIconKind.TagsOutlined: return typeof(TagsOutlined);
            case AntDesignIconKind.BorderBottomOutlined: return typeof(BorderBottomOutlined);
            case AntDesignIconKind.ArrowUpOutlined: return typeof(ArrowUpOutlined);
            case AntDesignIconKind.GooglePlusOutlined: return typeof(GooglePlusOutlined);
            case AntDesignIconKind.FolderAddOutlined: return typeof(FolderAddOutlined);
            default: throw new InvalidOperationException($"Icon kind {kind} does not exist");
        }
    }

    private static Icon CreateIconChunk7(AntDesignIconKind kind)
    {
        return kind switch
        {
            AntDesignIconKind.FundViewOutlined => new FundViewOutlined(),
            AntDesignIconKind.RestOutlined => new RestOutlined(),
            AntDesignIconKind.ContactsOutlined => new ContactsOutlined(),
            AntDesignIconKind.FormOutlined => new FormOutlined(),
            AntDesignIconKind.StarOutlined => new StarOutlined(),
            AntDesignIconKind.TrademarkCircleOutlined => new TrademarkCircleOutlined(),
            AntDesignIconKind.WindowUnpinOutlined => new WindowUnpinOutlined(),
            AntDesignIconKind.SunOutlined => new SunOutlined(),
            AntDesignIconKind.OrderedListOutlined => new OrderedListOutlined(),
            AntDesignIconKind.ExperimentOutlined => new ExperimentOutlined(),
            AntDesignIconKind.DeleteColumnOutlined => new DeleteColumnOutlined(),
            AntDesignIconKind.EditOutlined => new EditOutlined(),
            AntDesignIconKind.ApiOutlined => new ApiOutlined(),
            AntDesignIconKind.TwitchOutlined => new TwitchOutlined(),
            AntDesignIconKind.WindowsOutlined => new WindowsOutlined(),
            AntDesignIconKind.BugOutlined => new BugOutlined(),
            AntDesignIconKind.YoutubeOutlined => new YoutubeOutlined(),
            AntDesignIconKind.UnlockOutlined => new UnlockOutlined(),
            AntDesignIconKind.WindowPinOutlined => new WindowPinOutlined(),
            AntDesignIconKind.ToTopOutlined => new ToTopOutlined(),
            AntDesignIconKind.BorderLeftOutlined => new BorderLeftOutlined(),
            AntDesignIconKind.CompassOutlined => new CompassOutlined(),
            AntDesignIconKind.AlibabaOutlined => new AlibabaOutlined(),
            AntDesignIconKind.PlusCircleOutlined => new PlusCircleOutlined(),
            AntDesignIconKind.MenuFoldOutlined => new MenuFoldOutlined(),
            AntDesignIconKind.SisternodeOutlined => new SisternodeOutlined(),
            AntDesignIconKind.WomanOutlined => new WomanOutlined(),
            AntDesignIconKind.BankOutlined => new BankOutlined(),
            AntDesignIconKind.CreditCardOutlined => new CreditCardOutlined(),
            AntDesignIconKind.FileMarkdownOutlined => new FileMarkdownOutlined(),
            AntDesignIconKind.VerticalAlignBottomOutlined => new VerticalAlignBottomOutlined(),
            AntDesignIconKind.GatewayOutlined => new GatewayOutlined(),
            AntDesignIconKind.SelectOutlined => new SelectOutlined(),
            AntDesignIconKind.YahooOutlined => new YahooOutlined(),
            AntDesignIconKind.Loading3QuartersOutlined => new Loading3QuartersOutlined(),
            AntDesignIconKind.WindowCloseOutlined => new WindowCloseOutlined(),
            AntDesignIconKind.SketchOutlined => new SketchOutlined(),
            AntDesignIconKind.AudioOutlined => new AudioOutlined(),
            AntDesignIconKind.DeleteOutlined => new DeleteOutlined(),
            AntDesignIconKind.RedditOutlined => new RedditOutlined(),
            AntDesignIconKind.AlipayOutlined => new AlipayOutlined(),
            AntDesignIconKind.PicRightOutlined => new PicRightOutlined(),
            AntDesignIconKind.RetweetOutlined => new RetweetOutlined(),
            AntDesignIconKind.SkinOutlined => new SkinOutlined(),
            AntDesignIconKind.PhoneOutlined => new PhoneOutlined(),
            AntDesignIconKind.WechatWorkOutlined => new WechatWorkOutlined(),
            AntDesignIconKind.SyncOutlined => new SyncOutlined(),
            AntDesignIconKind.EyeOutlined => new EyeOutlined(),
            AntDesignIconKind.MobileOutlined => new MobileOutlined(),
            AntDesignIconKind.InsuranceOutlined => new InsuranceOutlined(),
            AntDesignIconKind.CodepenOutlined => new CodepenOutlined(),
            AntDesignIconKind.DribbbleOutlined => new DribbbleOutlined(),
            AntDesignIconKind.GiftOutlined => new GiftOutlined(),
            AntDesignIconKind.LineChartOutlined => new LineChartOutlined(),
            AntDesignIconKind.CarOutlined => new CarOutlined(),
            AntDesignIconKind.CiCircleOutlined => new CiCircleOutlined(),
            AntDesignIconKind.WeiboSquareOutlined => new WeiboSquareOutlined(),
            AntDesignIconKind.ThunderboltOutlined => new ThunderboltOutlined(),
            AntDesignIconKind.ProfileOutlined => new ProfileOutlined(),
            AntDesignIconKind.TagsOutlined => new TagsOutlined(),
            AntDesignIconKind.BorderBottomOutlined => new BorderBottomOutlined(),
            AntDesignIconKind.ArrowUpOutlined => new ArrowUpOutlined(),
            AntDesignIconKind.GooglePlusOutlined => new GooglePlusOutlined(),
            AntDesignIconKind.FolderAddOutlined => new FolderAddOutlined(),
            _ => throw new InvalidOperationException($"Icon kind {kind} does not exist")
        };
    }

    [UnconditionalSuppressMessage("Trimming", "IL2063",
        Justification = "Every switch arm returns typeof(...) for a generated icon class with a public parameterless constructor.")]
    [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
    private static Type GetIconTypeChunk8(AntDesignIconKind kind)
    {
        switch (kind)
        {
            case AntDesignIconKind.ScheduleOutlined: return typeof(ScheduleOutlined);
            case AntDesignIconKind.LinkedinOutlined: return typeof(LinkedinOutlined);
            case AntDesignIconKind.FieldBinaryOutlined: return typeof(FieldBinaryOutlined);
            case AntDesignIconKind.QqOutlined: return typeof(QqOutlined);
            case AntDesignIconKind.ShakeOutlined: return typeof(ShakeOutlined);
            case AntDesignIconKind.UndoOutlined: return typeof(UndoOutlined);
            case AntDesignIconKind.CheckBoldOutlined: return typeof(CheckBoldOutlined);
            case AntDesignIconKind.RobotOutlined: return typeof(RobotOutlined);
            case AntDesignIconKind.TwitterOutlined: return typeof(TwitterOutlined);
            case AntDesignIconKind.FilterOutlined: return typeof(FilterOutlined);
            case AntDesignIconKind.FieldStringOutlined: return typeof(FieldStringOutlined);
            case AntDesignIconKind.ItalicOutlined: return typeof(ItalicOutlined);
            case AntDesignIconKind.DiscordOutlined: return typeof(DiscordOutlined);
            case AntDesignIconKind.CommentOutlined: return typeof(CommentOutlined);
            case AntDesignIconKind.CalendarOutlined: return typeof(CalendarOutlined);
            case AntDesignIconKind.VideoCameraOutlined: return typeof(VideoCameraOutlined);
            case AntDesignIconKind.ArrowLeftOutlined: return typeof(ArrowLeftOutlined);
            case AntDesignIconKind.AlignCenterOutlined: return typeof(AlignCenterOutlined);
            case AntDesignIconKind.EllipsisOutlined: return typeof(EllipsisOutlined);
            case AntDesignIconKind.MinusCircleOutlined: return typeof(MinusCircleOutlined);
            case AntDesignIconKind.VideoCameraAddOutlined: return typeof(VideoCameraAddOutlined);
            case AntDesignIconKind.SplitCellsOutlined: return typeof(SplitCellsOutlined);
            case AntDesignIconKind.MergeCellsOutlined: return typeof(MergeCellsOutlined);
            case AntDesignIconKind.BorderHorizontalOutlined: return typeof(BorderHorizontalOutlined);
            case AntDesignIconKind.LaptopOutlined: return typeof(LaptopOutlined);
            case AntDesignIconKind.DotChartOutlined: return typeof(DotChartOutlined);
            case AntDesignIconKind.CaretLeftOutlined: return typeof(CaretLeftOutlined);
            case AntDesignIconKind.FontColorsOutlined: return typeof(FontColorsOutlined);
            case AntDesignIconKind.CloseSquareOutlined: return typeof(CloseSquareOutlined);
            case AntDesignIconKind.SlackOutlined: return typeof(SlackOutlined);
            case AntDesignIconKind.ZhihuOutlined: return typeof(ZhihuOutlined);
            case AntDesignIconKind.CloudOutlined: return typeof(CloudOutlined);
            case AntDesignIconKind.IeOutlined: return typeof(IeOutlined);
            case AntDesignIconKind.ReadOutlined: return typeof(ReadOutlined);
            case AntDesignIconKind.InsertRowBelowOutlined: return typeof(InsertRowBelowOutlined);
            case AntDesignIconKind.BarsOutlined: return typeof(BarsOutlined);
            case AntDesignIconKind.VerticalAlignTopOutlined: return typeof(VerticalAlignTopOutlined);
            case AntDesignIconKind.UploadOutlined: return typeof(UploadOutlined);
            case AntDesignIconKind.InteractionOutlined: return typeof(InteractionOutlined);
            case AntDesignIconKind.PauseOutlined: return typeof(PauseOutlined);
            case AntDesignIconKind.DoubleRightOutlined: return typeof(DoubleRightOutlined);
            case AntDesignIconKind.PropertySafetyOutlined: return typeof(PropertySafetyOutlined);
            case AntDesignIconKind.ForwardOutlined: return typeof(ForwardOutlined);
            case AntDesignIconKind.CodeSandboxOutlined: return typeof(CodeSandboxOutlined);
            case AntDesignIconKind.BaiduOutlined: return typeof(BaiduOutlined);
            case AntDesignIconKind.RightSquareOutlined: return typeof(RightSquareOutlined);
            case AntDesignIconKind.SkypeOutlined: return typeof(SkypeOutlined);
            case AntDesignIconKind.RocketOutlined: return typeof(RocketOutlined);
            case AntDesignIconKind.RotateRightOutlined: return typeof(RotateRightOutlined);
            case AntDesignIconKind.TabletOutlined: return typeof(TabletOutlined);
            case AntDesignIconKind.EnterOutlined: return typeof(EnterOutlined);
            case AntDesignIconKind.DotNetOutlined: return typeof(DotNetOutlined);
            case AntDesignIconKind.InsertRowRightOutlined: return typeof(InsertRowRightOutlined);
            case AntDesignIconKind.CloudDownloadOutlined: return typeof(CloudDownloadOutlined);
            case AntDesignIconKind.FundProjectionScreenOutlined: return typeof(FundProjectionScreenOutlined);
            case AntDesignIconKind.MenuOutlined: return typeof(MenuOutlined);
            case AntDesignIconKind.PushpinOutlined: return typeof(PushpinOutlined);
            case AntDesignIconKind.MergeOutlined: return typeof(MergeOutlined);
            case AntDesignIconKind.HddOutlined: return typeof(HddOutlined);
            case AntDesignIconKind.ChromeOutlined: return typeof(ChromeOutlined);
            case AntDesignIconKind.ShoppingCartOutlined: return typeof(ShoppingCartOutlined);
            case AntDesignIconKind.SortAscendingOutlined: return typeof(SortAscendingOutlined);
            case AntDesignIconKind.CalculatorOutlined: return typeof(CalculatorOutlined);
            case AntDesignIconKind.WindowRestoreOutlined: return typeof(WindowRestoreOutlined);
            default: throw new InvalidOperationException($"Icon kind {kind} does not exist");
        }
    }

    private static Icon CreateIconChunk8(AntDesignIconKind kind)
    {
        return kind switch
        {
            AntDesignIconKind.ScheduleOutlined => new ScheduleOutlined(),
            AntDesignIconKind.LinkedinOutlined => new LinkedinOutlined(),
            AntDesignIconKind.FieldBinaryOutlined => new FieldBinaryOutlined(),
            AntDesignIconKind.QqOutlined => new QqOutlined(),
            AntDesignIconKind.ShakeOutlined => new ShakeOutlined(),
            AntDesignIconKind.UndoOutlined => new UndoOutlined(),
            AntDesignIconKind.CheckBoldOutlined => new CheckBoldOutlined(),
            AntDesignIconKind.RobotOutlined => new RobotOutlined(),
            AntDesignIconKind.TwitterOutlined => new TwitterOutlined(),
            AntDesignIconKind.FilterOutlined => new FilterOutlined(),
            AntDesignIconKind.FieldStringOutlined => new FieldStringOutlined(),
            AntDesignIconKind.ItalicOutlined => new ItalicOutlined(),
            AntDesignIconKind.DiscordOutlined => new DiscordOutlined(),
            AntDesignIconKind.CommentOutlined => new CommentOutlined(),
            AntDesignIconKind.CalendarOutlined => new CalendarOutlined(),
            AntDesignIconKind.VideoCameraOutlined => new VideoCameraOutlined(),
            AntDesignIconKind.ArrowLeftOutlined => new ArrowLeftOutlined(),
            AntDesignIconKind.AlignCenterOutlined => new AlignCenterOutlined(),
            AntDesignIconKind.EllipsisOutlined => new EllipsisOutlined(),
            AntDesignIconKind.MinusCircleOutlined => new MinusCircleOutlined(),
            AntDesignIconKind.VideoCameraAddOutlined => new VideoCameraAddOutlined(),
            AntDesignIconKind.SplitCellsOutlined => new SplitCellsOutlined(),
            AntDesignIconKind.MergeCellsOutlined => new MergeCellsOutlined(),
            AntDesignIconKind.BorderHorizontalOutlined => new BorderHorizontalOutlined(),
            AntDesignIconKind.LaptopOutlined => new LaptopOutlined(),
            AntDesignIconKind.DotChartOutlined => new DotChartOutlined(),
            AntDesignIconKind.CaretLeftOutlined => new CaretLeftOutlined(),
            AntDesignIconKind.FontColorsOutlined => new FontColorsOutlined(),
            AntDesignIconKind.CloseSquareOutlined => new CloseSquareOutlined(),
            AntDesignIconKind.SlackOutlined => new SlackOutlined(),
            AntDesignIconKind.ZhihuOutlined => new ZhihuOutlined(),
            AntDesignIconKind.CloudOutlined => new CloudOutlined(),
            AntDesignIconKind.IeOutlined => new IeOutlined(),
            AntDesignIconKind.ReadOutlined => new ReadOutlined(),
            AntDesignIconKind.InsertRowBelowOutlined => new InsertRowBelowOutlined(),
            AntDesignIconKind.BarsOutlined => new BarsOutlined(),
            AntDesignIconKind.VerticalAlignTopOutlined => new VerticalAlignTopOutlined(),
            AntDesignIconKind.UploadOutlined => new UploadOutlined(),
            AntDesignIconKind.InteractionOutlined => new InteractionOutlined(),
            AntDesignIconKind.PauseOutlined => new PauseOutlined(),
            AntDesignIconKind.DoubleRightOutlined => new DoubleRightOutlined(),
            AntDesignIconKind.PropertySafetyOutlined => new PropertySafetyOutlined(),
            AntDesignIconKind.ForwardOutlined => new ForwardOutlined(),
            AntDesignIconKind.CodeSandboxOutlined => new CodeSandboxOutlined(),
            AntDesignIconKind.BaiduOutlined => new BaiduOutlined(),
            AntDesignIconKind.RightSquareOutlined => new RightSquareOutlined(),
            AntDesignIconKind.SkypeOutlined => new SkypeOutlined(),
            AntDesignIconKind.RocketOutlined => new RocketOutlined(),
            AntDesignIconKind.RotateRightOutlined => new RotateRightOutlined(),
            AntDesignIconKind.TabletOutlined => new TabletOutlined(),
            AntDesignIconKind.EnterOutlined => new EnterOutlined(),
            AntDesignIconKind.DotNetOutlined => new DotNetOutlined(),
            AntDesignIconKind.InsertRowRightOutlined => new InsertRowRightOutlined(),
            AntDesignIconKind.CloudDownloadOutlined => new CloudDownloadOutlined(),
            AntDesignIconKind.FundProjectionScreenOutlined => new FundProjectionScreenOutlined(),
            AntDesignIconKind.MenuOutlined => new MenuOutlined(),
            AntDesignIconKind.PushpinOutlined => new PushpinOutlined(),
            AntDesignIconKind.MergeOutlined => new MergeOutlined(),
            AntDesignIconKind.HddOutlined => new HddOutlined(),
            AntDesignIconKind.ChromeOutlined => new ChromeOutlined(),
            AntDesignIconKind.ShoppingCartOutlined => new ShoppingCartOutlined(),
            AntDesignIconKind.SortAscendingOutlined => new SortAscendingOutlined(),
            AntDesignIconKind.CalculatorOutlined => new CalculatorOutlined(),
            AntDesignIconKind.WindowRestoreOutlined => new WindowRestoreOutlined(),
            _ => throw new InvalidOperationException($"Icon kind {kind} does not exist")
        };
    }

    [UnconditionalSuppressMessage("Trimming", "IL2063",
        Justification = "Every switch arm returns typeof(...) for a generated icon class with a public parameterless constructor.")]
    [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
    private static Type GetIconTypeChunk9(AntDesignIconKind kind)
    {
        switch (kind)
        {
            case AntDesignIconKind.AppleOutlined: return typeof(AppleOutlined);
            case AntDesignIconKind.BlockOutlined: return typeof(BlockOutlined);
            case AntDesignIconKind.ManOutlined: return typeof(ManOutlined);
            case AntDesignIconKind.MedicineBoxOutlined: return typeof(MedicineBoxOutlined);
            case AntDesignIconKind.ProjectOutlined: return typeof(ProjectOutlined);
            case AntDesignIconKind.HistoryOutlined: return typeof(HistoryOutlined);
            case AntDesignIconKind.FolderOutlined: return typeof(FolderOutlined);
            case AntDesignIconKind.ArrowsAltOutlined: return typeof(ArrowsAltOutlined);
            case AntDesignIconKind.TaobaoOutlined: return typeof(TaobaoOutlined);
            case AntDesignIconKind.FilePptOutlined: return typeof(FilePptOutlined);
            case AntDesignIconKind.BgColorsOutlined: return typeof(BgColorsOutlined);
            case AntDesignIconKind.LeftOutlined: return typeof(LeftOutlined);
            case AntDesignIconKind.FilePdfOutlined: return typeof(FilePdfOutlined);
            case AntDesignIconKind.MonitorOutlined: return typeof(MonitorOutlined);
            case AntDesignIconKind.MinusOutlined: return typeof(MinusOutlined);
            case AntDesignIconKind.MoonOutlined: return typeof(MoonOutlined);
            case AntDesignIconKind.ExclamationOutlined: return typeof(ExclamationOutlined);
            case AntDesignIconKind.SwapOutlined: return typeof(SwapOutlined);
            case AntDesignIconKind.BehanceOutlined: return typeof(BehanceOutlined);
            case AntDesignIconKind.CustomerServiceOutlined: return typeof(CustomerServiceOutlined);
            case AntDesignIconKind.TikTokOutlined: return typeof(TikTokOutlined);
            case AntDesignIconKind.FieldTimeOutlined: return typeof(FieldTimeOutlined);
            case AntDesignIconKind.WindowMaximizedOutlined: return typeof(WindowMaximizedOutlined);
            case AntDesignIconKind.RadiusUpleftOutlined: return typeof(RadiusUpleftOutlined);
            case AntDesignIconKind.ExpandAltOutlined: return typeof(ExpandAltOutlined);
            case AntDesignIconKind.CloudServerOutlined: return typeof(CloudServerOutlined);
            case AntDesignIconKind.LayoutOutlined: return typeof(LayoutOutlined);
            case AntDesignIconKind.ClockCircleOutlined: return typeof(ClockCircleOutlined);
            case AntDesignIconKind.AlignLeftOutlined: return typeof(AlignLeftOutlined);
            case AntDesignIconKind.HeartOutlined: return typeof(HeartOutlined);
            case AntDesignIconKind.CaretDownFilled: return typeof(CaretDownFilled);
            case AntDesignIconKind.CodepenSquareFilled: return typeof(CodepenSquareFilled);
            case AntDesignIconKind.LikeFilled: return typeof(LikeFilled);
            case AntDesignIconKind.PauseCircleFilled: return typeof(PauseCircleFilled);
            case AntDesignIconKind.CheckSquareFilled: return typeof(CheckSquareFilled);
            case AntDesignIconKind.SwitcherFilled: return typeof(SwitcherFilled);
            case AntDesignIconKind.QqSquareFilled: return typeof(QqSquareFilled);
            case AntDesignIconKind.MoneyCollectFilled: return typeof(MoneyCollectFilled);
            case AntDesignIconKind.BulbFilled: return typeof(BulbFilled);
            case AntDesignIconKind.FileUnknownFilled: return typeof(FileUnknownFilled);
            case AntDesignIconKind.AppstoreFilled: return typeof(AppstoreFilled);
            case AntDesignIconKind.FileExcelFilled: return typeof(FileExcelFilled);
            case AntDesignIconKind.SoundFilled: return typeof(SoundFilled);
            case AntDesignIconKind.CodeSandboxSquareFilled: return typeof(CodeSandboxSquareFilled);
            case AntDesignIconKind.LeftCircleFilled: return typeof(LeftCircleFilled);
            case AntDesignIconKind.PlayCircleFilled: return typeof(PlayCircleFilled);
            case AntDesignIconKind.FileZipFilled: return typeof(FileZipFilled);
            case AntDesignIconKind.HourglassFilled: return typeof(HourglassFilled);
            case AntDesignIconKind.IeCircleFilled: return typeof(IeCircleFilled);
            case AntDesignIconKind.TaobaoSquareFilled: return typeof(TaobaoSquareFilled);
            case AntDesignIconKind.GooglePlusSquareFilled: return typeof(GooglePlusSquareFilled);
            case AntDesignIconKind.ZhihuSquareFilled: return typeof(ZhihuSquareFilled);
            case AntDesignIconKind.HighlightFilled: return typeof(HighlightFilled);
            case AntDesignIconKind.ReconciliationFilled: return typeof(ReconciliationFilled);
            case AntDesignIconKind.BilibiliFilled: return typeof(BilibiliFilled);
            case AntDesignIconKind.DingtalkSquareFilled: return typeof(DingtalkSquareFilled);
            case AntDesignIconKind.AndroidFilled: return typeof(AndroidFilled);
            case AntDesignIconKind.HomeFilled: return typeof(HomeFilled);
            case AntDesignIconKind.PoundCircleFilled: return typeof(PoundCircleFilled);
            case AntDesignIconKind.ShopFilled: return typeof(ShopFilled);
            case AntDesignIconKind.AlertFilled: return typeof(AlertFilled);
            case AntDesignIconKind.GitlabFilled: return typeof(GitlabFilled);
            case AntDesignIconKind.SlidersFilled: return typeof(SlidersFilled);
            case AntDesignIconKind.DollarCircleFilled: return typeof(DollarCircleFilled);
            default: throw new InvalidOperationException($"Icon kind {kind} does not exist");
        }
    }

    private static Icon CreateIconChunk9(AntDesignIconKind kind)
    {
        return kind switch
        {
            AntDesignIconKind.AppleOutlined => new AppleOutlined(),
            AntDesignIconKind.BlockOutlined => new BlockOutlined(),
            AntDesignIconKind.ManOutlined => new ManOutlined(),
            AntDesignIconKind.MedicineBoxOutlined => new MedicineBoxOutlined(),
            AntDesignIconKind.ProjectOutlined => new ProjectOutlined(),
            AntDesignIconKind.HistoryOutlined => new HistoryOutlined(),
            AntDesignIconKind.FolderOutlined => new FolderOutlined(),
            AntDesignIconKind.ArrowsAltOutlined => new ArrowsAltOutlined(),
            AntDesignIconKind.TaobaoOutlined => new TaobaoOutlined(),
            AntDesignIconKind.FilePptOutlined => new FilePptOutlined(),
            AntDesignIconKind.BgColorsOutlined => new BgColorsOutlined(),
            AntDesignIconKind.LeftOutlined => new LeftOutlined(),
            AntDesignIconKind.FilePdfOutlined => new FilePdfOutlined(),
            AntDesignIconKind.MonitorOutlined => new MonitorOutlined(),
            AntDesignIconKind.MinusOutlined => new MinusOutlined(),
            AntDesignIconKind.MoonOutlined => new MoonOutlined(),
            AntDesignIconKind.ExclamationOutlined => new ExclamationOutlined(),
            AntDesignIconKind.SwapOutlined => new SwapOutlined(),
            AntDesignIconKind.BehanceOutlined => new BehanceOutlined(),
            AntDesignIconKind.CustomerServiceOutlined => new CustomerServiceOutlined(),
            AntDesignIconKind.TikTokOutlined => new TikTokOutlined(),
            AntDesignIconKind.FieldTimeOutlined => new FieldTimeOutlined(),
            AntDesignIconKind.WindowMaximizedOutlined => new WindowMaximizedOutlined(),
            AntDesignIconKind.RadiusUpleftOutlined => new RadiusUpleftOutlined(),
            AntDesignIconKind.ExpandAltOutlined => new ExpandAltOutlined(),
            AntDesignIconKind.CloudServerOutlined => new CloudServerOutlined(),
            AntDesignIconKind.LayoutOutlined => new LayoutOutlined(),
            AntDesignIconKind.ClockCircleOutlined => new ClockCircleOutlined(),
            AntDesignIconKind.AlignLeftOutlined => new AlignLeftOutlined(),
            AntDesignIconKind.HeartOutlined => new HeartOutlined(),
            AntDesignIconKind.CaretDownFilled => new CaretDownFilled(),
            AntDesignIconKind.CodepenSquareFilled => new CodepenSquareFilled(),
            AntDesignIconKind.LikeFilled => new LikeFilled(),
            AntDesignIconKind.PauseCircleFilled => new PauseCircleFilled(),
            AntDesignIconKind.CheckSquareFilled => new CheckSquareFilled(),
            AntDesignIconKind.SwitcherFilled => new SwitcherFilled(),
            AntDesignIconKind.QqSquareFilled => new QqSquareFilled(),
            AntDesignIconKind.MoneyCollectFilled => new MoneyCollectFilled(),
            AntDesignIconKind.BulbFilled => new BulbFilled(),
            AntDesignIconKind.FileUnknownFilled => new FileUnknownFilled(),
            AntDesignIconKind.AppstoreFilled => new AppstoreFilled(),
            AntDesignIconKind.FileExcelFilled => new FileExcelFilled(),
            AntDesignIconKind.SoundFilled => new SoundFilled(),
            AntDesignIconKind.CodeSandboxSquareFilled => new CodeSandboxSquareFilled(),
            AntDesignIconKind.LeftCircleFilled => new LeftCircleFilled(),
            AntDesignIconKind.PlayCircleFilled => new PlayCircleFilled(),
            AntDesignIconKind.FileZipFilled => new FileZipFilled(),
            AntDesignIconKind.HourglassFilled => new HourglassFilled(),
            AntDesignIconKind.IeCircleFilled => new IeCircleFilled(),
            AntDesignIconKind.TaobaoSquareFilled => new TaobaoSquareFilled(),
            AntDesignIconKind.GooglePlusSquareFilled => new GooglePlusSquareFilled(),
            AntDesignIconKind.ZhihuSquareFilled => new ZhihuSquareFilled(),
            AntDesignIconKind.HighlightFilled => new HighlightFilled(),
            AntDesignIconKind.ReconciliationFilled => new ReconciliationFilled(),
            AntDesignIconKind.BilibiliFilled => new BilibiliFilled(),
            AntDesignIconKind.DingtalkSquareFilled => new DingtalkSquareFilled(),
            AntDesignIconKind.AndroidFilled => new AndroidFilled(),
            AntDesignIconKind.HomeFilled => new HomeFilled(),
            AntDesignIconKind.PoundCircleFilled => new PoundCircleFilled(),
            AntDesignIconKind.ShopFilled => new ShopFilled(),
            AntDesignIconKind.AlertFilled => new AlertFilled(),
            AntDesignIconKind.GitlabFilled => new GitlabFilled(),
            AntDesignIconKind.SlidersFilled => new SlidersFilled(),
            AntDesignIconKind.DollarCircleFilled => new DollarCircleFilled(),
            _ => throw new InvalidOperationException($"Icon kind {kind} does not exist")
        };
    }

    [UnconditionalSuppressMessage("Trimming", "IL2063",
        Justification = "Every switch arm returns typeof(...) for a generated icon class with a public parameterless constructor.")]
    [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
    private static Type GetIconTypeChunk10(AntDesignIconKind kind)
    {
        switch (kind)
        {
            case AntDesignIconKind.ShoppingFilled: return typeof(ShoppingFilled);
            case AntDesignIconKind.FileWordFilled: return typeof(FileWordFilled);
            case AntDesignIconKind.FunnelPlotFilled: return typeof(FunnelPlotFilled);
            case AntDesignIconKind.GithubFilled: return typeof(GithubFilled);
            case AntDesignIconKind.UsbFilled: return typeof(UsbFilled);
            case AntDesignIconKind.EuroCircleFilled: return typeof(EuroCircleFilled);
            case AntDesignIconKind.GoldenFilled: return typeof(GoldenFilled);
            case AntDesignIconKind.TagFilled: return typeof(TagFilled);
            case AntDesignIconKind.UpSquareFilled: return typeof(UpSquareFilled);
            case AntDesignIconKind.MediumCircleFilled: return typeof(MediumCircleFilled);
            case AntDesignIconKind.DownSquareFilled: return typeof(DownSquareFilled);
            case AntDesignIconKind.FileAddFilled: return typeof(FileAddFilled);
            case AntDesignIconKind.PlusSquareFilled: return typeof(PlusSquareFilled);
            case AntDesignIconKind.DatabaseFilled: return typeof(DatabaseFilled);
            case AntDesignIconKind.SketchSquareFilled: return typeof(SketchSquareFilled);
            case AntDesignIconKind.FileFilled: return typeof(FileFilled);
            case AntDesignIconKind.AccountBookFilled: return typeof(AccountBookFilled);
            case AntDesignIconKind.ControlFilled: return typeof(ControlFilled);
            case AntDesignIconKind.RedEnvelopeFilled: return typeof(RedEnvelopeFilled);
            case AntDesignIconKind.SignalFilled: return typeof(SignalFilled);
            case AntDesignIconKind.FitToWindowFilled: return typeof(FitToWindowFilled);
            case AntDesignIconKind.RedditSquareFilled: return typeof(RedditSquareFilled);
            case AntDesignIconKind.BoxPlotFilled: return typeof(BoxPlotFilled);
            case AntDesignIconKind.DropboxCircleFilled: return typeof(DropboxCircleFilled);
            case AntDesignIconKind.FileTextFilled: return typeof(FileTextFilled);
            case AntDesignIconKind.FolderOpenFilled: return typeof(FolderOpenFilled);
            case AntDesignIconKind.BuildFilled: return typeof(BuildFilled);
            case AntDesignIconKind.XFilled: return typeof(XFilled);
            case AntDesignIconKind.BehanceSquareFilled: return typeof(BehanceSquareFilled);
            case AntDesignIconKind.GiteeFilled: return typeof(GiteeFilled);
            case AntDesignIconKind.QuestionCircleFilled: return typeof(QuestionCircleFilled);
            case AntDesignIconKind.LockFilled: return typeof(LockFilled);
            case AntDesignIconKind.FireFilled: return typeof(FireFilled);
            case AntDesignIconKind.WeiboCircleFilled: return typeof(WeiboCircleFilled);
            case AntDesignIconKind.DislikeFilled: return typeof(DislikeFilled);
            case AntDesignIconKind.InstagramFilled: return typeof(InstagramFilled);
            case AntDesignIconKind.IdcardFilled: return typeof(IdcardFilled);
            case AntDesignIconKind.ProductFilled: return typeof(ProductFilled);
            case AntDesignIconKind.MehFilled: return typeof(MehFilled);
            case AntDesignIconKind.DiffFilled: return typeof(DiffFilled);
            case AntDesignIconKind.MinusSquareFilled: return typeof(MinusSquareFilled);
            case AntDesignIconKind.CaretUpFilled: return typeof(CaretUpFilled);
            case AntDesignIconKind.CloseCircleFilled: return typeof(CloseCircleFilled);
            case AntDesignIconKind.MailFilled: return typeof(MailFilled);
            case AntDesignIconKind.BookFilled: return typeof(BookFilled);
            case AntDesignIconKind.WalletFilled: return typeof(WalletFilled);
            case AntDesignIconKind.FileImageFilled: return typeof(FileImageFilled);
            case AntDesignIconKind.BellFilled: return typeof(BellFilled);
            case AntDesignIconKind.DashboardFilled: return typeof(DashboardFilled);
            case AntDesignIconKind.CodeFilled: return typeof(CodeFilled);
            case AntDesignIconKind.AlipayCircleFilled: return typeof(AlipayCircleFilled);
            case AntDesignIconKind.AliwangwangFilled: return typeof(AliwangwangFilled);
            case AntDesignIconKind.CarryOutFilled: return typeof(CarryOutFilled);
            case AntDesignIconKind.FlagFilled: return typeof(FlagFilled);
            case AntDesignIconKind.SnippetsFilled: return typeof(SnippetsFilled);
            case AntDesignIconKind.StopFilled: return typeof(StopFilled);
            case AntDesignIconKind.RightCircleFilled: return typeof(RightCircleFilled);
            case AntDesignIconKind.StepForwardFilled: return typeof(StepForwardFilled);
            case AntDesignIconKind.MutedFilled: return typeof(MutedFilled);
            case AntDesignIconKind.ContainerFilled: return typeof(ContainerFilled);
            case AntDesignIconKind.AmazonSquareFilled: return typeof(AmazonSquareFilled);
            case AntDesignIconKind.SlackSquareFilled: return typeof(SlackSquareFilled);
            case AntDesignIconKind.FrownFilled: return typeof(FrownFilled);
            case AntDesignIconKind.ToolFilled: return typeof(ToolFilled);
            default: throw new InvalidOperationException($"Icon kind {kind} does not exist");
        }
    }

    private static Icon CreateIconChunk10(AntDesignIconKind kind)
    {
        return kind switch
        {
            AntDesignIconKind.ShoppingFilled => new ShoppingFilled(),
            AntDesignIconKind.FileWordFilled => new FileWordFilled(),
            AntDesignIconKind.FunnelPlotFilled => new FunnelPlotFilled(),
            AntDesignIconKind.GithubFilled => new GithubFilled(),
            AntDesignIconKind.UsbFilled => new UsbFilled(),
            AntDesignIconKind.EuroCircleFilled => new EuroCircleFilled(),
            AntDesignIconKind.GoldenFilled => new GoldenFilled(),
            AntDesignIconKind.TagFilled => new TagFilled(),
            AntDesignIconKind.UpSquareFilled => new UpSquareFilled(),
            AntDesignIconKind.MediumCircleFilled => new MediumCircleFilled(),
            AntDesignIconKind.DownSquareFilled => new DownSquareFilled(),
            AntDesignIconKind.FileAddFilled => new FileAddFilled(),
            AntDesignIconKind.PlusSquareFilled => new PlusSquareFilled(),
            AntDesignIconKind.DatabaseFilled => new DatabaseFilled(),
            AntDesignIconKind.SketchSquareFilled => new SketchSquareFilled(),
            AntDesignIconKind.FileFilled => new FileFilled(),
            AntDesignIconKind.AccountBookFilled => new AccountBookFilled(),
            AntDesignIconKind.ControlFilled => new ControlFilled(),
            AntDesignIconKind.RedEnvelopeFilled => new RedEnvelopeFilled(),
            AntDesignIconKind.SignalFilled => new SignalFilled(),
            AntDesignIconKind.FitToWindowFilled => new FitToWindowFilled(),
            AntDesignIconKind.RedditSquareFilled => new RedditSquareFilled(),
            AntDesignIconKind.BoxPlotFilled => new BoxPlotFilled(),
            AntDesignIconKind.DropboxCircleFilled => new DropboxCircleFilled(),
            AntDesignIconKind.FileTextFilled => new FileTextFilled(),
            AntDesignIconKind.FolderOpenFilled => new FolderOpenFilled(),
            AntDesignIconKind.BuildFilled => new BuildFilled(),
            AntDesignIconKind.XFilled => new XFilled(),
            AntDesignIconKind.BehanceSquareFilled => new BehanceSquareFilled(),
            AntDesignIconKind.GiteeFilled => new GiteeFilled(),
            AntDesignIconKind.QuestionCircleFilled => new QuestionCircleFilled(),
            AntDesignIconKind.LockFilled => new LockFilled(),
            AntDesignIconKind.FireFilled => new FireFilled(),
            AntDesignIconKind.WeiboCircleFilled => new WeiboCircleFilled(),
            AntDesignIconKind.DislikeFilled => new DislikeFilled(),
            AntDesignIconKind.InstagramFilled => new InstagramFilled(),
            AntDesignIconKind.IdcardFilled => new IdcardFilled(),
            AntDesignIconKind.ProductFilled => new ProductFilled(),
            AntDesignIconKind.MehFilled => new MehFilled(),
            AntDesignIconKind.DiffFilled => new DiffFilled(),
            AntDesignIconKind.MinusSquareFilled => new MinusSquareFilled(),
            AntDesignIconKind.CaretUpFilled => new CaretUpFilled(),
            AntDesignIconKind.CloseCircleFilled => new CloseCircleFilled(),
            AntDesignIconKind.MailFilled => new MailFilled(),
            AntDesignIconKind.BookFilled => new BookFilled(),
            AntDesignIconKind.WalletFilled => new WalletFilled(),
            AntDesignIconKind.FileImageFilled => new FileImageFilled(),
            AntDesignIconKind.BellFilled => new BellFilled(),
            AntDesignIconKind.DashboardFilled => new DashboardFilled(),
            AntDesignIconKind.CodeFilled => new CodeFilled(),
            AntDesignIconKind.AlipayCircleFilled => new AlipayCircleFilled(),
            AntDesignIconKind.AliwangwangFilled => new AliwangwangFilled(),
            AntDesignIconKind.CarryOutFilled => new CarryOutFilled(),
            AntDesignIconKind.FlagFilled => new FlagFilled(),
            AntDesignIconKind.SnippetsFilled => new SnippetsFilled(),
            AntDesignIconKind.StopFilled => new StopFilled(),
            AntDesignIconKind.RightCircleFilled => new RightCircleFilled(),
            AntDesignIconKind.StepForwardFilled => new StepForwardFilled(),
            AntDesignIconKind.MutedFilled => new MutedFilled(),
            AntDesignIconKind.ContainerFilled => new ContainerFilled(),
            AntDesignIconKind.AmazonSquareFilled => new AmazonSquareFilled(),
            AntDesignIconKind.SlackSquareFilled => new SlackSquareFilled(),
            AntDesignIconKind.FrownFilled => new FrownFilled(),
            AntDesignIconKind.ToolFilled => new ToolFilled(),
            _ => throw new InvalidOperationException($"Icon kind {kind} does not exist")
        };
    }

    [UnconditionalSuppressMessage("Trimming", "IL2063",
        Justification = "Every switch arm returns typeof(...) for a generated icon class with a public parameterless constructor.")]
    [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
    private static Type GetIconTypeChunk11(AntDesignIconKind kind)
    {
        switch (kind)
        {
            case AntDesignIconKind.SafetyCertificateFilled: return typeof(SafetyCertificateFilled);
            case AntDesignIconKind.DribbbleSquareFilled: return typeof(DribbbleSquareFilled);
            case AntDesignIconKind.GoogleSquareFilled: return typeof(GoogleSquareFilled);
            case AntDesignIconKind.TwitterCircleFilled: return typeof(TwitterCircleFilled);
            case AntDesignIconKind.CaretRightFilled: return typeof(CaretRightFilled);
            case AntDesignIconKind.TrophyFilled: return typeof(TrophyFilled);
            case AntDesignIconKind.WarningFilled: return typeof(WarningFilled);
            case AntDesignIconKind.PieChartFilled: return typeof(PieChartFilled);
            case AntDesignIconKind.SecurityScanFilled: return typeof(SecurityScanFilled);
            case AntDesignIconKind.SpotifyFilled: return typeof(SpotifyFilled);
            case AntDesignIconKind.InfoCircleFilled: return typeof(InfoCircleFilled);
            case AntDesignIconKind.EyeInvisibleFilled: return typeof(EyeInvisibleFilled);
            case AntDesignIconKind.LeftSquareFilled: return typeof(LeftSquareFilled);
            case AntDesignIconKind.CopyFilled: return typeof(CopyFilled);
            case AntDesignIconKind.GoldFilled: return typeof(GoldFilled);
            case AntDesignIconKind.CodeSandboxCircleFilled: return typeof(CodeSandboxCircleFilled);
            case AntDesignIconKind.FundFilled: return typeof(FundFilled);
            case AntDesignIconKind.SignatureFilled: return typeof(SignatureFilled);
            case AntDesignIconKind.PlaySquareFilled: return typeof(PlaySquareFilled);
            case AntDesignIconKind.FileExclamationFilled: return typeof(FileExclamationFilled);
            case AntDesignIconKind.CodepenCircleFilled: return typeof(CodepenCircleFilled);
            case AntDesignIconKind.FastBackwardFilled: return typeof(FastBackwardFilled);
            case AntDesignIconKind.ImageFilled: return typeof(ImageFilled);
            case AntDesignIconKind.EnvironmentFilled: return typeof(EnvironmentFilled);
            case AntDesignIconKind.CheckCircleFilled: return typeof(CheckCircleFilled);
            case AntDesignIconKind.Html5Filled: return typeof(Html5Filled);
            case AntDesignIconKind.SaveFilled: return typeof(SaveFilled);
            case AntDesignIconKind.SmileFilled: return typeof(SmileFilled);
            case AntDesignIconKind.QqCircleFilled: return typeof(QqCircleFilled);
            case AntDesignIconKind.SettingFilled: return typeof(SettingFilled);
            case AntDesignIconKind.FastForwardFilled: return typeof(FastForwardFilled);
            case AntDesignIconKind.MessageFilled: return typeof(MessageFilled);
            case AntDesignIconKind.CopyrightCircleFilled: return typeof(CopyrightCircleFilled);
            case AntDesignIconKind.DingtalkCircleFilled: return typeof(DingtalkCircleFilled);
            case AntDesignIconKind.CrownFilled: return typeof(CrownFilled);
            case AntDesignIconKind.BackwardFilled: return typeof(BackwardFilled);
            case AntDesignIconKind.YuqueFilled: return typeof(YuqueFilled);
            case AntDesignIconKind.NotificationFilled: return typeof(NotificationFilled);
            case AntDesignIconKind.PictureFilled: return typeof(PictureFilled);
            case AntDesignIconKind.IeSquareFilled: return typeof(IeSquareFilled);
            case AntDesignIconKind.TaobaoCircleFilled: return typeof(TaobaoCircleFilled);
            case AntDesignIconKind.GooglePlusCircleFilled: return typeof(GooglePlusCircleFilled);
            case AntDesignIconKind.FacebookFilled: return typeof(FacebookFilled);
            case AntDesignIconKind.ZhihuCircleFilled: return typeof(ZhihuCircleFilled);
            case AntDesignIconKind.CameraFilled: return typeof(CameraFilled);
            case AntDesignIconKind.PrinterFilled: return typeof(PrinterFilled);
            case AntDesignIconKind.PinterestFilled: return typeof(PinterestFilled);
            case AntDesignIconKind.TruckFilled: return typeof(TruckFilled);
            case AntDesignIconKind.PayCircleFilled: return typeof(PayCircleFilled);
            case AntDesignIconKind.WechatFilled: return typeof(WechatFilled);
            case AntDesignIconKind.OpenAIFilled: return typeof(OpenAIFilled);
            case AntDesignIconKind.FormatPainterFilled: return typeof(FormatPainterFilled);
            case AntDesignIconKind.MediumSquareFilled: return typeof(MediumSquareFilled);
            case AntDesignIconKind.UpCircleFilled: return typeof(UpCircleFilled);
            case AntDesignIconKind.ExclamationCircleFilled: return typeof(ExclamationCircleFilled);
            case AntDesignIconKind.MacCommandFilled: return typeof(MacCommandFilled);
            case AntDesignIconKind.DownCircleFilled: return typeof(DownCircleFilled);
            case AntDesignIconKind.StepBackwardFilled: return typeof(StepBackwardFilled);
            case AntDesignIconKind.RestFilled: return typeof(RestFilled);
            case AntDesignIconKind.ContactsFilled: return typeof(ContactsFilled);
            case AntDesignIconKind.StarFilled: return typeof(StarFilled);
            case AntDesignIconKind.TrademarkCircleFilled: return typeof(TrademarkCircleFilled);
            case AntDesignIconKind.SunFilled: return typeof(SunFilled);
            case AntDesignIconKind.ExperimentFilled: return typeof(ExperimentFilled);
            default: throw new InvalidOperationException($"Icon kind {kind} does not exist");
        }
    }

    private static Icon CreateIconChunk11(AntDesignIconKind kind)
    {
        return kind switch
        {
            AntDesignIconKind.SafetyCertificateFilled => new SafetyCertificateFilled(),
            AntDesignIconKind.DribbbleSquareFilled => new DribbbleSquareFilled(),
            AntDesignIconKind.GoogleSquareFilled => new GoogleSquareFilled(),
            AntDesignIconKind.TwitterCircleFilled => new TwitterCircleFilled(),
            AntDesignIconKind.CaretRightFilled => new CaretRightFilled(),
            AntDesignIconKind.TrophyFilled => new TrophyFilled(),
            AntDesignIconKind.WarningFilled => new WarningFilled(),
            AntDesignIconKind.PieChartFilled => new PieChartFilled(),
            AntDesignIconKind.SecurityScanFilled => new SecurityScanFilled(),
            AntDesignIconKind.SpotifyFilled => new SpotifyFilled(),
            AntDesignIconKind.InfoCircleFilled => new InfoCircleFilled(),
            AntDesignIconKind.EyeInvisibleFilled => new EyeInvisibleFilled(),
            AntDesignIconKind.LeftSquareFilled => new LeftSquareFilled(),
            AntDesignIconKind.CopyFilled => new CopyFilled(),
            AntDesignIconKind.GoldFilled => new GoldFilled(),
            AntDesignIconKind.CodeSandboxCircleFilled => new CodeSandboxCircleFilled(),
            AntDesignIconKind.FundFilled => new FundFilled(),
            AntDesignIconKind.SignatureFilled => new SignatureFilled(),
            AntDesignIconKind.PlaySquareFilled => new PlaySquareFilled(),
            AntDesignIconKind.FileExclamationFilled => new FileExclamationFilled(),
            AntDesignIconKind.CodepenCircleFilled => new CodepenCircleFilled(),
            AntDesignIconKind.FastBackwardFilled => new FastBackwardFilled(),
            AntDesignIconKind.ImageFilled => new ImageFilled(),
            AntDesignIconKind.EnvironmentFilled => new EnvironmentFilled(),
            AntDesignIconKind.CheckCircleFilled => new CheckCircleFilled(),
            AntDesignIconKind.Html5Filled => new Html5Filled(),
            AntDesignIconKind.SaveFilled => new SaveFilled(),
            AntDesignIconKind.SmileFilled => new SmileFilled(),
            AntDesignIconKind.QqCircleFilled => new QqCircleFilled(),
            AntDesignIconKind.SettingFilled => new SettingFilled(),
            AntDesignIconKind.FastForwardFilled => new FastForwardFilled(),
            AntDesignIconKind.MessageFilled => new MessageFilled(),
            AntDesignIconKind.CopyrightCircleFilled => new CopyrightCircleFilled(),
            AntDesignIconKind.DingtalkCircleFilled => new DingtalkCircleFilled(),
            AntDesignIconKind.CrownFilled => new CrownFilled(),
            AntDesignIconKind.BackwardFilled => new BackwardFilled(),
            AntDesignIconKind.YuqueFilled => new YuqueFilled(),
            AntDesignIconKind.NotificationFilled => new NotificationFilled(),
            AntDesignIconKind.PictureFilled => new PictureFilled(),
            AntDesignIconKind.IeSquareFilled => new IeSquareFilled(),
            AntDesignIconKind.TaobaoCircleFilled => new TaobaoCircleFilled(),
            AntDesignIconKind.GooglePlusCircleFilled => new GooglePlusCircleFilled(),
            AntDesignIconKind.FacebookFilled => new FacebookFilled(),
            AntDesignIconKind.ZhihuCircleFilled => new ZhihuCircleFilled(),
            AntDesignIconKind.CameraFilled => new CameraFilled(),
            AntDesignIconKind.PrinterFilled => new PrinterFilled(),
            AntDesignIconKind.PinterestFilled => new PinterestFilled(),
            AntDesignIconKind.TruckFilled => new TruckFilled(),
            AntDesignIconKind.PayCircleFilled => new PayCircleFilled(),
            AntDesignIconKind.WechatFilled => new WechatFilled(),
            AntDesignIconKind.OpenAIFilled => new OpenAIFilled(),
            AntDesignIconKind.FormatPainterFilled => new FormatPainterFilled(),
            AntDesignIconKind.MediumSquareFilled => new MediumSquareFilled(),
            AntDesignIconKind.UpCircleFilled => new UpCircleFilled(),
            AntDesignIconKind.ExclamationCircleFilled => new ExclamationCircleFilled(),
            AntDesignIconKind.MacCommandFilled => new MacCommandFilled(),
            AntDesignIconKind.DownCircleFilled => new DownCircleFilled(),
            AntDesignIconKind.StepBackwardFilled => new StepBackwardFilled(),
            AntDesignIconKind.RestFilled => new RestFilled(),
            AntDesignIconKind.ContactsFilled => new ContactsFilled(),
            AntDesignIconKind.StarFilled => new StarFilled(),
            AntDesignIconKind.TrademarkCircleFilled => new TrademarkCircleFilled(),
            AntDesignIconKind.SunFilled => new SunFilled(),
            AntDesignIconKind.ExperimentFilled => new ExperimentFilled(),
            _ => throw new InvalidOperationException($"Icon kind {kind} does not exist")
        };
    }

    [UnconditionalSuppressMessage("Trimming", "IL2063",
        Justification = "Every switch arm returns typeof(...) for a generated icon class with a public parameterless constructor.")]
    [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
    private static Type GetIconTypeChunk12(AntDesignIconKind kind)
    {
        switch (kind)
        {
            case AntDesignIconKind.EditFilled: return typeof(EditFilled);
            case AntDesignIconKind.RedditCircleFilled: return typeof(RedditCircleFilled);
            case AntDesignIconKind.ApiFilled: return typeof(ApiFilled);
            case AntDesignIconKind.TwitchFilled: return typeof(TwitchFilled);
            case AntDesignIconKind.WindowsFilled: return typeof(WindowsFilled);
            case AntDesignIconKind.BugFilled: return typeof(BugFilled);
            case AntDesignIconKind.YoutubeFilled: return typeof(YoutubeFilled);
            case AntDesignIconKind.DropboxSquareFilled: return typeof(DropboxSquareFilled);
            case AntDesignIconKind.UnlockFilled: return typeof(UnlockFilled);
            case AntDesignIconKind.CompassFilled: return typeof(CompassFilled);
            case AntDesignIconKind.PlusCircleFilled: return typeof(PlusCircleFilled);
            case AntDesignIconKind.BankFilled: return typeof(BankFilled);
            case AntDesignIconKind.CreditCardFilled: return typeof(CreditCardFilled);
            case AntDesignIconKind.SketchCircleFilled: return typeof(SketchCircleFilled);
            case AntDesignIconKind.FileMarkdownFilled: return typeof(FileMarkdownFilled);
            case AntDesignIconKind.YahooFilled: return typeof(YahooFilled);
            case AntDesignIconKind.AudioFilled: return typeof(AudioFilled);
            case AntDesignIconKind.DeleteFilled: return typeof(DeleteFilled);
            case AntDesignIconKind.SkinFilled: return typeof(SkinFilled);
            case AntDesignIconKind.PhoneFilled: return typeof(PhoneFilled);
            case AntDesignIconKind.WechatWorkFilled: return typeof(WechatWorkFilled);
            case AntDesignIconKind.EyeFilled: return typeof(EyeFilled);
            case AntDesignIconKind.MobileFilled: return typeof(MobileFilled);
            case AntDesignIconKind.BehanceCircleFilled: return typeof(BehanceCircleFilled);
            case AntDesignIconKind.InsuranceFilled: return typeof(InsuranceFilled);
            case AntDesignIconKind.GiftFilled: return typeof(GiftFilled);
            case AntDesignIconKind.CarFilled: return typeof(CarFilled);
            case AntDesignIconKind.CiCircleFilled: return typeof(CiCircleFilled);
            case AntDesignIconKind.WeiboSquareFilled: return typeof(WeiboSquareFilled);
            case AntDesignIconKind.ThunderboltFilled: return typeof(ThunderboltFilled);
            case AntDesignIconKind.ProfileFilled: return typeof(ProfileFilled);
            case AntDesignIconKind.TagsFilled: return typeof(TagsFilled);
            case AntDesignIconKind.FolderAddFilled: return typeof(FolderAddFilled);
            case AntDesignIconKind.ScheduleFilled: return typeof(ScheduleFilled);
            case AntDesignIconKind.LinkedinFilled: return typeof(LinkedinFilled);
            case AntDesignIconKind.RobotFilled: return typeof(RobotFilled);
            case AntDesignIconKind.FilterFilled: return typeof(FilterFilled);
            case AntDesignIconKind.DiscordFilled: return typeof(DiscordFilled);
            case AntDesignIconKind.CalendarFilled: return typeof(CalendarFilled);
            case AntDesignIconKind.VideoCameraFilled: return typeof(VideoCameraFilled);
            case AntDesignIconKind.MinusCircleFilled: return typeof(MinusCircleFilled);
            case AntDesignIconKind.CaretLeftFilled: return typeof(CaretLeftFilled);
            case AntDesignIconKind.CloseSquareFilled: return typeof(CloseSquareFilled);
            case AntDesignIconKind.CloudFilled: return typeof(CloudFilled);
            case AntDesignIconKind.AlipaySquareFilled: return typeof(AlipaySquareFilled);
            case AntDesignIconKind.ReadFilled: return typeof(ReadFilled);
            case AntDesignIconKind.InteractionFilled: return typeof(InteractionFilled);
            case AntDesignIconKind.PropertySafetyFilled: return typeof(PropertySafetyFilled);
            case AntDesignIconKind.ForwardFilled: return typeof(ForwardFilled);
            case AntDesignIconKind.RightSquareFilled: return typeof(RightSquareFilled);
            case AntDesignIconKind.SkypeFilled: return typeof(SkypeFilled);
            case AntDesignIconKind.RocketFilled: return typeof(RocketFilled);
            case AntDesignIconKind.AmazonCircleFilled: return typeof(AmazonCircleFilled);
            case AntDesignIconKind.SlackCircleFilled: return typeof(SlackCircleFilled);
            case AntDesignIconKind.TabletFilled: return typeof(TabletFilled);
            case AntDesignIconKind.PushpinFilled: return typeof(PushpinFilled);
            case AntDesignIconKind.MergeFilled: return typeof(MergeFilled);
            case AntDesignIconKind.HddFilled: return typeof(HddFilled);
            case AntDesignIconKind.ChromeFilled: return typeof(ChromeFilled);
            case AntDesignIconKind.CalculatorFilled: return typeof(CalculatorFilled);
            case AntDesignIconKind.AppleFilled: return typeof(AppleFilled);
            case AntDesignIconKind.MedicineBoxFilled: return typeof(MedicineBoxFilled);
            case AntDesignIconKind.ProjectFilled: return typeof(ProjectFilled);
            case AntDesignIconKind.FolderFilled: return typeof(FolderFilled);
            default: throw new InvalidOperationException($"Icon kind {kind} does not exist");
        }
    }

    private static Icon CreateIconChunk12(AntDesignIconKind kind)
    {
        return kind switch
        {
            AntDesignIconKind.EditFilled => new EditFilled(),
            AntDesignIconKind.RedditCircleFilled => new RedditCircleFilled(),
            AntDesignIconKind.ApiFilled => new ApiFilled(),
            AntDesignIconKind.TwitchFilled => new TwitchFilled(),
            AntDesignIconKind.WindowsFilled => new WindowsFilled(),
            AntDesignIconKind.BugFilled => new BugFilled(),
            AntDesignIconKind.YoutubeFilled => new YoutubeFilled(),
            AntDesignIconKind.DropboxSquareFilled => new DropboxSquareFilled(),
            AntDesignIconKind.UnlockFilled => new UnlockFilled(),
            AntDesignIconKind.CompassFilled => new CompassFilled(),
            AntDesignIconKind.PlusCircleFilled => new PlusCircleFilled(),
            AntDesignIconKind.BankFilled => new BankFilled(),
            AntDesignIconKind.CreditCardFilled => new CreditCardFilled(),
            AntDesignIconKind.SketchCircleFilled => new SketchCircleFilled(),
            AntDesignIconKind.FileMarkdownFilled => new FileMarkdownFilled(),
            AntDesignIconKind.YahooFilled => new YahooFilled(),
            AntDesignIconKind.AudioFilled => new AudioFilled(),
            AntDesignIconKind.DeleteFilled => new DeleteFilled(),
            AntDesignIconKind.SkinFilled => new SkinFilled(),
            AntDesignIconKind.PhoneFilled => new PhoneFilled(),
            AntDesignIconKind.WechatWorkFilled => new WechatWorkFilled(),
            AntDesignIconKind.EyeFilled => new EyeFilled(),
            AntDesignIconKind.MobileFilled => new MobileFilled(),
            AntDesignIconKind.BehanceCircleFilled => new BehanceCircleFilled(),
            AntDesignIconKind.InsuranceFilled => new InsuranceFilled(),
            AntDesignIconKind.GiftFilled => new GiftFilled(),
            AntDesignIconKind.CarFilled => new CarFilled(),
            AntDesignIconKind.CiCircleFilled => new CiCircleFilled(),
            AntDesignIconKind.WeiboSquareFilled => new WeiboSquareFilled(),
            AntDesignIconKind.ThunderboltFilled => new ThunderboltFilled(),
            AntDesignIconKind.ProfileFilled => new ProfileFilled(),
            AntDesignIconKind.TagsFilled => new TagsFilled(),
            AntDesignIconKind.FolderAddFilled => new FolderAddFilled(),
            AntDesignIconKind.ScheduleFilled => new ScheduleFilled(),
            AntDesignIconKind.LinkedinFilled => new LinkedinFilled(),
            AntDesignIconKind.RobotFilled => new RobotFilled(),
            AntDesignIconKind.FilterFilled => new FilterFilled(),
            AntDesignIconKind.DiscordFilled => new DiscordFilled(),
            AntDesignIconKind.CalendarFilled => new CalendarFilled(),
            AntDesignIconKind.VideoCameraFilled => new VideoCameraFilled(),
            AntDesignIconKind.MinusCircleFilled => new MinusCircleFilled(),
            AntDesignIconKind.CaretLeftFilled => new CaretLeftFilled(),
            AntDesignIconKind.CloseSquareFilled => new CloseSquareFilled(),
            AntDesignIconKind.CloudFilled => new CloudFilled(),
            AntDesignIconKind.AlipaySquareFilled => new AlipaySquareFilled(),
            AntDesignIconKind.ReadFilled => new ReadFilled(),
            AntDesignIconKind.InteractionFilled => new InteractionFilled(),
            AntDesignIconKind.PropertySafetyFilled => new PropertySafetyFilled(),
            AntDesignIconKind.ForwardFilled => new ForwardFilled(),
            AntDesignIconKind.RightSquareFilled => new RightSquareFilled(),
            AntDesignIconKind.SkypeFilled => new SkypeFilled(),
            AntDesignIconKind.RocketFilled => new RocketFilled(),
            AntDesignIconKind.AmazonCircleFilled => new AmazonCircleFilled(),
            AntDesignIconKind.SlackCircleFilled => new SlackCircleFilled(),
            AntDesignIconKind.TabletFilled => new TabletFilled(),
            AntDesignIconKind.PushpinFilled => new PushpinFilled(),
            AntDesignIconKind.MergeFilled => new MergeFilled(),
            AntDesignIconKind.HddFilled => new HddFilled(),
            AntDesignIconKind.ChromeFilled => new ChromeFilled(),
            AntDesignIconKind.CalculatorFilled => new CalculatorFilled(),
            AntDesignIconKind.AppleFilled => new AppleFilled(),
            AntDesignIconKind.MedicineBoxFilled => new MedicineBoxFilled(),
            AntDesignIconKind.ProjectFilled => new ProjectFilled(),
            AntDesignIconKind.FolderFilled => new FolderFilled(),
            _ => throw new InvalidOperationException($"Icon kind {kind} does not exist")
        };
    }

    [UnconditionalSuppressMessage("Trimming", "IL2063",
        Justification = "Every switch arm returns typeof(...) for a generated icon class with a public parameterless constructor.")]
    [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
    private static Type GetIconTypeChunk13(AntDesignIconKind kind)
    {
        switch (kind)
        {
            case AntDesignIconKind.FilePptFilled: return typeof(FilePptFilled);
            case AntDesignIconKind.FilePdfFilled: return typeof(FilePdfFilled);
            case AntDesignIconKind.TwitterSquareFilled: return typeof(TwitterSquareFilled);
            case AntDesignIconKind.GoogleCircleFilled: return typeof(GoogleCircleFilled);
            case AntDesignIconKind.DribbbleCircleFilled: return typeof(DribbbleCircleFilled);
            case AntDesignIconKind.MoonFilled: return typeof(MoonFilled);
            case AntDesignIconKind.CustomerServiceFilled: return typeof(CustomerServiceFilled);
            case AntDesignIconKind.TikTokFilled: return typeof(TikTokFilled);
            case AntDesignIconKind.LayoutFilled: return typeof(LayoutFilled);
            case AntDesignIconKind.ClockCircleFilled: return typeof(ClockCircleFilled);
            case AntDesignIconKind.HeartFilled: return typeof(HeartFilled);
            default: throw new InvalidOperationException($"Icon kind {kind} does not exist");
        }
    }

    private static Icon CreateIconChunk13(AntDesignIconKind kind)
    {
        return kind switch
        {
            AntDesignIconKind.FilePptFilled => new FilePptFilled(),
            AntDesignIconKind.FilePdfFilled => new FilePdfFilled(),
            AntDesignIconKind.TwitterSquareFilled => new TwitterSquareFilled(),
            AntDesignIconKind.GoogleCircleFilled => new GoogleCircleFilled(),
            AntDesignIconKind.DribbbleCircleFilled => new DribbbleCircleFilled(),
            AntDesignIconKind.MoonFilled => new MoonFilled(),
            AntDesignIconKind.CustomerServiceFilled => new CustomerServiceFilled(),
            AntDesignIconKind.TikTokFilled => new TikTokFilled(),
            AntDesignIconKind.LayoutFilled => new LayoutFilled(),
            AntDesignIconKind.ClockCircleFilled => new ClockCircleFilled(),
            AntDesignIconKind.HeartFilled => new HeartFilled(),
            _ => throw new InvalidOperationException($"Icon kind {kind} does not exist")
        };
    }
}
