﻿using System;
using Codartis.SoftVis.Modeling;

namespace Codartis.SoftVis.Diagramming
{
    public abstract class DiagramShape
    {
        public UmlModelElement ModelElement { get; private set; }

        protected DiagramShape(UmlModelElement modelElement)
        {
            if (modelElement == null) throw new ArgumentNullException(nameof(modelElement));

            ModelElement = modelElement;
        }
    }
}