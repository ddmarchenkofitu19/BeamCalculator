using BeamCalculator.Models.Section;
using SkiaSharp;

namespace BeamCalculator.Helpers.Drawing;


public class DrawMapper<T> where T : IDrawingOptions
{
    private Dictionary<string, Action<SectionModel, SKCanvas, T>> genericMap = new Dictionary<string, Action<SectionModel, SKCanvas, T>>();


    public DrawMapper()
    { }

    public Action<SectionModel, SKCanvas, T> this[string key]
    {
        set => genericMap[key] = (section, canvas, options) =>
        value?.Invoke(section, canvas, options);
    }


    public bool DrawLayer(string key, SectionModel section, SKCanvas canvas, T options)
    {
        var action = Get(key);
        
        if (action == null)
            return false;

        action.Invoke(section, canvas, options);

        return true;
    }

    public Action<SectionModel, SKCanvas, T> Get(string key)
    {
        genericMap.TryGetValue(key, out var action);
        return action;
    }
}
