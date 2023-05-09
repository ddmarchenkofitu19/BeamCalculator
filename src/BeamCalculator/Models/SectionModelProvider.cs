using BeamCalculator.Models.Section;

namespace BeamCalculator.Models;


public class SectionModelProvider
{
	public SectionModelProvider()
	{

	}

    public CommonSectionModel GetSectionModel(SectionTypes type)
	{
        var s = MauiProgram.Services;
        switch (type)
        {
            case SectionTypes.None:
                return null;

            case SectionTypes.Rectangle:
                return s.GetRequiredService<RectangleSectionModel>();
            case SectionTypes.TProfileSymmetric:
                return s.GetRequiredService<TProfileSymmetricSectionModel>();
            case SectionTypes.TProfile:
                return s.GetRequiredService<TProfileSectionModel>();
            case SectionTypes.IProfile:
                return s.GetRequiredService<IProfileSectionModel>();
            case SectionTypes.RectangleTubing:
                return s.GetRequiredService<RectangleTubingSectionModel>();
            case SectionTypes.UProfile:
                return s.GetRequiredService<UProfileSectionModel>();
            case SectionTypes.RoundTubing:
                return s.GetRequiredService<RoundTubingSectionModel>();
            case SectionTypes.XProfile:
                return s.GetRequiredService<XProfileSectionModel>();
            case SectionTypes.LProfile:
                return s.GetRequiredService<LProfileSectionModel>();

            default:
                return null;
        }
    }
}
