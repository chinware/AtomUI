using AtomUI.Theme.TokenSystem;

namespace AtomUI.Theme;

public class DynamicTheme : Theme
{
    private bool _needBroadcast;
    private IList<string> _algorithms;
    private IDictionary<string, string> _globalTokens;
    private IList<ControlTokenConfigInfo> _controlTokenConfigInfos;

    public DynamicTheme(string id, string defFilePath)
        : base(id, defFilePath)
    {
        _algorithms              = new List<string>();
        _globalTokens            = new Dictionary<string, string>();
        _controlTokenConfigInfos = new List<ControlTokenConfigInfo>();
    }

    public override bool IsDynamic()
    {
        return true;
    }

    public IList<string> Algorithms
    {
        get => _algorithms;

        set
        {
            _algorithms    = value;
            _needBroadcast = true;
        }
    }

    public void AddAlgorithm(string algorithm)
    {
        if (!_algorithms.Contains(algorithm))
        {
            _algorithms.Add(algorithm);
            _needBroadcast = true;
        }
    }

    public IDictionary<string, string> GlobalTokens
    {
        get => _globalTokens;

        set
        {
            _globalTokens  = value;
            _needBroadcast = true;
        }
    }

    public void AddGlobalToken(string name, string value)
    {
        _globalTokens.Add(name, value);
        _needBroadcast = true;
    }

    public IList<ControlTokenConfigInfo> ControlTokens
    {
        get => _controlTokenConfigInfos;

        set
        {
            _controlTokenConfigInfos = value;
            _needBroadcast           = true;
        }
    }

    public void AddControlToken(ControlTokenConfigInfo tokenConfigInfo)
    {
        // 先不检查重复，原则上是不能重复的
        _controlTokenConfigInfos.Add(tokenConfigInfo);
        _needBroadcast = true;
    }

    internal override void NotifyLoadThemeDef()
    {
    }
}