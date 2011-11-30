﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RiaServicesContrib
{
    internal class InitializeAttribute : Attribute
    {
    }
    internal class DisposeAttribute : Attribute
    {
    }

    public partial class EntityGraph<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// Gets the source entity of this entity graph.
        /// </summary>
        public TEntity Source { get; private set; }
        protected IEntityGraphShape GraphShape { get; set; }
        /// <summary>
        /// Initializes a new instance of the EntityGraph class with given shape for given source entity.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="graphShape"></param>
        public EntityGraph(TEntity source, IEntityGraphShape graphShape)
        {
            if(source == null)
            {
                throw new ArgumentNullException("source", "The cosntructor argument 'source' can't be null");
            }
            if(graphShape == null)
            {
                throw new ArgumentNullException("graphShape", "The constructor argument 'graphShape' can't be null");
            }
            this.Source = source;
            this.GraphShape = graphShape;

            var type = this.GetType();
            var flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
            var constructors = type.GetMethods(flags).Where(m => m.IsDefined(typeof(InitializeAttribute), true));

            foreach(var constructor in constructors)
            {
                constructor.Invoke(this, new object[] { });
            }
        }

        private EntityRelationGraph<TEntity> _entityRelationGraph;
        protected EntityRelationGraph<TEntity> EntityRelationGraph
        {
            get
            {
                if(_entityRelationGraph == null)
                {
                    _entityRelationGraph = new EntityRelationGraph<TEntity>();
                    BuildEntityGraph(Source, _entityRelationGraph);
                }
                return _entityRelationGraph;
            }
            set
            {
                _entityRelationGraph = value;
            }
        }

        /// <summary>
        /// Returns the entity graph as defined by associations that are marked with the 'EntityGraphAttribute' attribute.
        /// The resulting graph consists of a list of GraphNodes. Each GraphNode has an element 'Node' of type 'T', 
        /// which represents the actual node, a set, SingleEdges, which correspond to EntityRefs, and a
        /// a set, ListEdges, which correspond to EntityCollections. 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="graph"></param>
        private void BuildEntityGraph(TEntity entity, EntityRelationGraph<TEntity> graph)
        {
            if(graph.Nodes.Any(n => n.Node == entity))
                return;
            EntityRelation<TEntity> node = new EntityRelation<TEntity>() { Node = entity };
            graph.Nodes.Add(node);

            foreach(PropertyInfo edge in GraphShape.OutEdges(entity))
            {
                if(typeof(IEnumerable).IsAssignableFrom(edge.PropertyType))
                {
                    var assocList = GraphShape.GetNodes(entity, edge);
                    if (assocList == null)
                    {
                        continue;
                    }
                    node.ListEdges.Add(edge, new List<TEntity>());
                    foreach(TEntity e in assocList)
                    {
                        if(e != null)
                        {
                            node.ListEdges[edge].Add(e);
                            BuildEntityGraph(e, graph);
                        }
                    }
                }
                else
                {
                    TEntity e = (TEntity)GraphShape.GetNode(entity, edge);
                    if(e != null)
                    {
                        node.SingleEdges.Add(edge, e);
                        BuildEntityGraph(e, graph);
                    }
                }
            }
        }

        /// <summary>
        /// Eventhandler that is called just before the EntityRelationGraph is reset.
        /// </summary>
        protected EventHandler<EventArgs> EntityRelationGraphResetting;
        /// <summary>
        /// Eventhandler that is called right after the EntityRelationGraph is reset.
        /// </summary>
        protected EventHandler<EventArgs> EntityRelationGraphResetted;

        protected void EntityRelationGraphReset()
        {
            if(EntityRelationGraphResetting != null)
            {
                EntityRelationGraphResetting(this, new EventArgs());
            }
            EntityRelationGraph = null;
            if(EntityRelationGraphResetted != null)
            {
                EntityRelationGraphResetted(this, new EventArgs());
            }
        }
    }
}