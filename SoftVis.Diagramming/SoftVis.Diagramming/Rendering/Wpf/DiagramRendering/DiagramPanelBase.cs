﻿using Codartis.SoftVis.Diagramming;
using Codartis.SoftVis.Rendering.Wpf.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Codartis.SoftVis.Rendering.Wpf.DiagramRendering.ShapeControls;

namespace Codartis.SoftVis.Rendering.Wpf.DiagramRendering
{
    /// <summary>
    /// Manages controls created for diagram shapes.
    /// </summary>
    public abstract class DiagramPanelBase : Panel
    {
        private readonly Dictionary<DiagramShape, DiagramShapeControlBase> _diagramShapeControls =
            new Dictionary<DiagramShape, DiagramShapeControlBase>();

        private Rect _contentRect;

        public static readonly DependencyProperty DiagramProperty =
            DependencyProperty.Register("Diagram", typeof(Diagram), typeof(DiagramPanelBase),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure, Diagram_PropertyChanged));

        public Diagram Diagram
        {
            get { return (Diagram)GetValue(DiagramProperty); }
            set { SetValue(DiagramProperty, value); }
        }

        private static void Diagram_PropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var diagramPanel = (DiagramPanelBase)obj;
            var diagram = (Diagram)e.NewValue;
            diagramPanel.AddDiagram(diagram);

            diagram.ShapeAdded += diagramPanel.OnShapeAdded;
            diagram.ShapeModified += diagramPanel.OnShapeModified;
            diagram.ShapeRemoved += diagramPanel.OnShapeRemoved;
            diagram.Cleared += diagramPanel.OnDiagramCleared;
        }

        protected Rect ContentRect
        {
            get { return _contentRect == Rect.Empty ? new Rect(0,0,0,0) : _contentRect ; }
            private set { _contentRect = value; }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            foreach (var child in Children.OfType<DiagramShapeControlBase>())
                child.Measure(availableSize);

            ContentRect = Children.OfType<DiagramShapeControlBase>().Select(i => i.Rect).Union();
            return ContentRect.Size;
        }

        private void AddDiagram(Diagram diagram)
        {
            AddAllGraphElements(diagram);
        }

        private void OnShapeAdded(object sender, DiagramShape shape)
        {
            if (_diagramShapeControls.ContainsKey(shape))
                return;

            if (shape is DiagramNode)
                CreateDiagramNodeControl((DiagramNode)shape);
            else if (shape is DiagramConnector)
                CreateDiagramConnectorControl((DiagramConnector)shape);
        }

        private void OnShapeModified(object sender, DiagramShape shape)
        {
        }

        private void OnShapeRemoved(object sender, DiagramShape shape)
        {
            if (!_diagramShapeControls.ContainsKey(shape))
                return;

            RemoveDiagramShapeControl(shape);
        }

        private void OnDiagramCleared(object sender, EventArgs e)
        {
            Children.Clear();
            _diagramShapeControls.Clear();
        }

        private void AddAllGraphElements(Diagram diagram)
        {
            foreach (var node in diagram.Nodes)
                CreateDiagramNodeControl(node);

            foreach (var connector in diagram.Connectors)
                CreateDiagramConnectorControl(connector);
        }

        private void CreateDiagramNodeControl(DiagramNode diagramNode)
        {
            var control = new DiagramNodeControl { DataContext = diagramNode };
            _diagramShapeControls.Add(diagramNode, control);
            Children.Add(control);
        }

        private void CreateDiagramConnectorControl(DiagramConnector diagramConnector)
        {
            var control = new DiagramConnectorControl { DataContext = diagramConnector };
            _diagramShapeControls.Add(diagramConnector, control);
            Children.Add(control);
        }

        private void RemoveDiagramShapeControl(DiagramShape diagramShape)
        {
            Children.Remove(_diagramShapeControls[diagramShape]);
            _diagramShapeControls.Remove(diagramShape);
        }
    }
}