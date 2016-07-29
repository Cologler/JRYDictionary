using System;

namespace JryDictionary.Controls.ThingPreview
{
    public interface IThingPreviewViewModel
    {
        Uri Cover { get; }

        string Name { get; }
    }
}