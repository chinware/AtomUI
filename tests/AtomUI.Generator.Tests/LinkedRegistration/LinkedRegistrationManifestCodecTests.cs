using AtomUI.Generator.LinkedRegistration;
using AtomUI.Generator.LinkedRegistration.Manifest;
using Shouldly;
using Xunit;

namespace AtomUI.Generator.Tests.LinkedRegistration;

public sealed class LinkedRegistrationManifestCodecTests
{
    [Fact]
    public void Package_Record_Round_Trips_Optional_PackageShared_Fragment()
    {
        var expected = new LinkedPackageManifestRecord(
            "Acme.Controls|扩展",
            "Acme.Controls",
            "Acme.Controls.ThemeManagerBuilderExtensions.UseControls",
            "Acme.Generated.LinkedRegistration.FullPackageFragment",
            "Register",
            "Acme.Generated.LinkedRegistration.PackageSharedFragment",
            "Add");

        var envelope = LinkedRegistrationManifestCodec.Encode(expected);

        envelope.Key.ShouldBe(LinkedRegistrationProtocol.PackageManifestKey);
        envelope.Value.ShouldStartWith("1|");
        envelope.Value.ShouldNotContain("扩展");
        RoundTrip(expected).ShouldBe(expected);
    }

    [Fact]
    public void Package_Record_Round_Trips_Without_PackageShared_Fragment()
    {
        var expected = new LinkedPackageManifestRecord(
            "Acme.Controls",
            "Acme.Controls",
            "Acme.Controls.ThemeManagerBuilderExtensions.UseControls",
            "Acme.Generated.LinkedRegistration.FullPackageFragment",
            "Register",
            null,
            null);

        RoundTrip(expected).ShouldBe(expected);
    }

    [Fact]
    public void Unit_And_ControlMap_Records_Round_Trip_Stable_Strings()
    {
        var unit = new LinkedUnitManifestRecord(
            "Acme.Controls",
            "Acme.Controls/DatePicker",
            "Acme.Generated.LinkedRegistration.Unit_DatePicker_0123",
            "Add");
        var controlMap = new LinkedControlMapManifestRecord(
            "Acme.Controls",
            "Acme.Controls.RangeDatePicker`1+Presenter",
            "Acme.Controls/DatePicker");

        RoundTrip(unit).ShouldBe(unit);
        RoundTrip(controlMap).ShouldBe(controlMap);
    }

    [Fact]
    public void Usage_Record_Round_Trips_Source_Locations()
    {
        foreach (var kind in new[]
                 {
                     LinkedUsageKind.Control,
                     LinkedUsageKind.UnitRoot,
                     LinkedUsageKind.PackageRoot,
                     LinkedUsageKind.Entry
                 })
        {
            var usage = new LinkedUsageManifestRecord(
                kind,
                "Acme.Controls/DatePicker",
                "Views/Main View.axaml",
                12,
                8);

            RoundTrip(usage).ShouldBe(usage);
        }
    }

    [Fact]
    public void Decoder_Rejects_Unknown_Major_Versions()
    {
        LinkedRegistrationManifestCodec.TryDecode(
            "AtomUI.Linked.Unit.v2",
            "2|Acme.Controls",
            out var record,
            out var error).ShouldBeFalse();

        record.ShouldBeNull();
        error.ShouldContain("major version '2'");
    }

    [Fact]
    public void Decoder_Rejects_Rejected_Record_Keys()
    {
        foreach (var key in new[]
                 {
                     "AtomUI.Linked.Asset.v1",
                     "AtomUI.Linked.AuxiliaryAsset.v1",
                     "AtomUI.Linked.Catalog.v1",
                     "AtomUI.Linked.Feature.v1",
                     "AtomUI.Linked.Entry.v1"
                 })
        {
            LinkedRegistrationManifestCodec.TryDecode(
                key,
                "1|Acme.Controls",
                out var record,
                out var error).ShouldBeFalse();

            record.ShouldBeNull();
            error.ShouldContain("not supported");
        }
    }

    [Fact]
    public void Decoder_Rejects_Malformed_Field_Counts()
    {
        LinkedRegistrationManifestCodec.TryDecode(
            LinkedRegistrationProtocol.UnitManifestKey,
            "1|Acme.Controls",
            out var record,
            out var error).ShouldBeFalse();

        record.ShouldBeNull();
        error.ShouldContain("field count");
    }

    [Theory]
    [InlineData("1||Acme.Controls||Acme.Full|Register||")]
    [InlineData("1|Acme.Controls|||Acme.Full|Register||")]
    [InlineData("1|Acme.Controls|Acme.Controls|||Register||")]
    [InlineData("1|Acme.Controls|Acme.Controls||Acme.Full|||")]
    public void Decoder_Rejects_Empty_Required_Package_Fields(string value)
    {
        LinkedRegistrationManifestCodec.TryDecode(
            LinkedRegistrationProtocol.PackageManifestKey,
            value,
            out var record,
            out var error).ShouldBeFalse();

        record.ShouldBeNull();
        error.ShouldContain("required field");
    }

    [Fact]
    public void Decoder_Allows_An_Empty_Package_Entry_Method()
    {
        LinkedRegistrationManifestCodec.TryDecode(
            LinkedRegistrationProtocol.PackageManifestKey,
            "1|Acme.Controls|Acme.Controls||Acme.Full|Register||",
            out var record,
            out var error).ShouldBeTrue(error);

        record.ShouldBeAssignableTo<LinkedPackageManifestRecord>()!
              .EntryMethodMetadataNames.ShouldBeEmpty();
    }

    [Fact]
    public void Package_entry_list_round_trips_through_the_single_codec()
    {
        var package = new LinkedPackageManifestRecord(
            "Acme.Controls",
            "Acme.Controls",
            "Acme.Controls.Entry.UseAllControls;Acme.Controls.Entry.UseControls",
            "Acme.Controls.Generated.Full",
            "Register",
            null,
            null);

        var envelope = LinkedRegistrationManifestCodec.Encode(package);

        LinkedRegistrationManifestCodec.TryDecode(
            envelope.Key,
            envelope.Value,
            out var record,
            out var error).ShouldBeTrue(error);
        record.ShouldBe(package);
    }

    [Theory]
    [InlineData("1||Acme.Controls%2FDatePicker|Acme.Unit|Register")]
    [InlineData("1|Acme.Controls||Acme.Unit|Register")]
    [InlineData("1|Acme.Controls|Acme.Controls%2FDatePicker||Register")]
    [InlineData("1|Acme.Controls|Acme.Controls%2FDatePicker|Acme.Unit|")]
    public void Decoder_Rejects_Empty_Required_Unit_Fields(string value)
    {
        LinkedRegistrationManifestCodec.TryDecode(
            LinkedRegistrationProtocol.UnitManifestKey,
            value,
            out var record,
            out var error).ShouldBeFalse();

        record.ShouldBeNull();
        error.ShouldContain("required field");
    }

    [Theory]
    [InlineData("1||Acme.Controls.DatePicker|Acme.Controls%2FDatePicker")]
    [InlineData("1|Acme.Controls||Acme.Controls%2FDatePicker")]
    [InlineData("1|Acme.Controls|Acme.Controls.DatePicker|")]
    public void Decoder_Rejects_Empty_Required_ControlMap_Fields(string value)
    {
        LinkedRegistrationManifestCodec.TryDecode(
            LinkedRegistrationProtocol.ControlMapManifestKey,
            value,
            out var record,
            out var error).ShouldBeFalse();

        record.ShouldBeNull();
        error.ShouldContain("required field");
    }

    [Theory]
    [InlineData("1|Control||Main.axaml|0|0")]
    [InlineData("1|Control|Acme.Controls.DatePicker||0|0")]
    public void Decoder_Rejects_Empty_Required_Usage_Fields(string value)
    {
        LinkedRegistrationManifestCodec.TryDecode(
            LinkedRegistrationProtocol.UsageManifestKey,
            value,
            out var record,
            out var error).ShouldBeFalse();

        record.ShouldBeNull();
        error.ShouldContain("required field");
    }

    [Fact]
    public void Decoder_Rejects_Unknown_Usage_Kinds_And_Invalid_Locations()
    {
        LinkedRegistrationManifestCodec.TryDecode(
            LinkedRegistrationProtocol.UsageManifestKey,
            "1|Feature|Acme.Controls|Main.axaml|0|0",
            out _,
            out var kindError).ShouldBeFalse();
        kindError.ShouldContain("unknown usage kind");

        LinkedRegistrationManifestCodec.TryDecode(
            LinkedRegistrationProtocol.UsageManifestKey,
            "1|Control|Acme.Controls.Button|Main.axaml|-1|0",
            out _,
            out var locationError).ShouldBeFalse();
        locationError.ShouldContain("invalid source location");
    }

    private static LinkedRegistrationManifestRecord RoundTrip(
        LinkedRegistrationManifestRecord expected)
    {
        var envelope = LinkedRegistrationManifestCodec.Encode(expected);
        LinkedRegistrationManifestCodec.TryDecode(
            envelope.Key,
            envelope.Value,
            out var decoded,
            out var error).ShouldBeTrue(error);
        return decoded.ShouldNotBeNull();
    }
}
