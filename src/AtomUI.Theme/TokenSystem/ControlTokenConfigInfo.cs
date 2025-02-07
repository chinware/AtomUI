namespace AtomUI.Theme.TokenSystem;

public class ControlTokenConfigInfo
{
    public bool UseAlgorithm { get; set; } = false;
    public string TokenId { get; set; } = string.Empty;
    public IDictionary<string, string> Tokens { get; set; }

    public ControlTokenConfigInfo()
    {
        Tokens = new Dictionary<string, string>();
    }
}