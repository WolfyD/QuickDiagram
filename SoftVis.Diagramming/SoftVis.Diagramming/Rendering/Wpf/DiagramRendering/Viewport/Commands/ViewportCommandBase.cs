﻿namespace Codartis.SoftVis.Rendering.Wpf.DiagramRendering.Viewport.Commands
{
    public abstract class ViewportCommandBase
    {
        public object Sender { get; private set; }

        protected ViewportCommandBase(object sender)
        {
            Sender = sender;
        }

        internal abstract void Execute(IDiagramViewport diagramViewport);
    }
}