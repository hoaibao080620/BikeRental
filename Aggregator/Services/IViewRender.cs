namespace Aggregator.Services;

public interface IViewRender
{
    Task<string> RenderPartialViewToString<TModel>(string name, TModel model);
}
