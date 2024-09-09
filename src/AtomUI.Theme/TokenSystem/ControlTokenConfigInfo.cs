namespace AtomUI.Theme.TokenSystem;

public class ControlTokenConfigInfo
{
    public ControlTokenConfigInfo()
    {
        ControlTokens = new Dictionary<string, string>();
    }

    public bool UseAlgorithm { get; set; } = false;
    public string TokenId { get; set; } = string.Empty;
    public IDictionary<string, string> ControlTokens { get; set; }
}