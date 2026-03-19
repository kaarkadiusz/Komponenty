using System;
using System.Collections.Generic;
using System.Text;

namespace Komponenty.Modal
{
    public class KAModalReference
    {
        internal KAModalReference(string sectionName, Type? componentType = null, IDictionary<string, object>? componentParameters = null, KAModalOptions? modalOptions = null)
        {
            SectionName = sectionName;
            ComponentType = componentType;
            ComponentParameters = componentParameters;
            ModalOptions = modalOptions;
        }

        public string SectionName { get; }
        public KAModal? ModalRef { get; internal set; }

        public Type? ComponentType { get; }
        public IDictionary<string, object>? ComponentParameters { get; }
        public KAModalOptions? ModalOptions { get; }

        public bool IsDynamicComponent => ComponentType is not null;
    }
}
