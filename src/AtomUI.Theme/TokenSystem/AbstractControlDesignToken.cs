using System.Reflection;
using Avalonia.Controls;

namespace AtomUI.Theme.TokenSystem;

/// <summary>
/// 所有的组件 Token 定义是除了全局的 Token 的之外的专属于当前的组件的 Token 值
/// </summary>
public abstract class AbstractControlDesignToken : AbstractDesignToken, IControlDesignToken
{
   protected AliasDesignToken _globalToken;
   
   public string Id { get; init; }

   public bool IsCustomTokenConfig { get; internal set; }
   public IList<string> CustomTokens { get; internal set; }

   protected AbstractControlDesignToken(string id)
   {
      Id = id;
      IsCustomTokenConfig = false;
      _globalToken = default!;
      CustomTokens = new List<string>();
   }

   public void AssignGlobalToken(AliasDesignToken globalToken)
   {
      _globalToken = globalToken;
   }

   public override void BuildResourceDictionary(IResourceDictionary dictionary)
   {
      ResourceDictionary tempDictionary = new ResourceDictionary();
      base.BuildResourceDictionary(tempDictionary);
      if (IsCustomTokenConfig) {
         _globalToken.BuildResourceDictionary(tempDictionary);
      }
      // 增加自己的命名空间，现在这种方法效率不是很高，需要优化
      // 暂时先用这种方案，后期有更好的方案再做调整
      foreach (var entry in tempDictionary) {
         var resourceName = new TokenResourceKey($"{Id}.{entry.Key}");
         dictionary.Add(resourceName, entry.Value);
      }
   }
   
   /// <summary>
   /// 一般 control token 尽量不继承, 先看看
   /// </summary>
   /// <param name="tokenName"></param>
   /// <returns></returns>
   public bool HasToken(string tokenName)
   {
      var type = GetType();
      return type.GetProperty(tokenName, BindingFlags.Instance | BindingFlags.Public) is not null;
   }
   
   internal virtual void CalculateFromAlias()
   {
   }
}