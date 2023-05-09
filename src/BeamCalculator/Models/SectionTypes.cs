namespace BeamCalculator.Models;


public enum SectionTypes
{
    None,

    Custom,

    Rectangle,
    TProfileSymmetric,
    TProfile,
    IProfile,
    RectangleTubing,
    UProfile,
    RoundTubing,
    XProfile,
    LProfile,
}

public static class SectionTypesExtensions
{
    public static string ToUserFriendlyString(this SectionTypes value)
    {
        switch (value)
        {
            case SectionTypes.None:
                return "";
            case SectionTypes.Custom:
                return "Custom";
            case SectionTypes.Rectangle:
                return "Rectangle";
            case SectionTypes.TProfileSymmetric:
                return "T-Beam symmetrical";
            case SectionTypes.TProfile:
                return "T-Beam";
            case SectionTypes.IProfile:
                return "I-Beam";
            case SectionTypes.RectangleTubing:
                return "Rectangle Tubing";
            case SectionTypes.UProfile:
                return "U-Beam";
            case SectionTypes.RoundTubing:
                return "Round Tubing";
            case SectionTypes.XProfile:
                return "X-Beam";
            case SectionTypes.LProfile:
                return "L-Beam";

            default:
                return "";
        }
    }
}
