﻿using System.ServiceModel.DomainServices.Client;

namespace RiaServicesContrib.DomainServices.Client
{
    public partial class EntityGraph
    {
        #region DetachEntityGraph
        /// <summary>
        /// Method that detaches a graph of associated entities from their respective entity sets.
        /// The graph of entities is defined by associations which are marked with an entity graph attribute of type 'GraphType'.
        /// </summary>
        /// <param name="entitySet"></param>
        public void DetachEntityGraph<TEntity>(EntitySet<TEntity> entitySet) where TEntity : Entity
        {
            GraphMap(e => DetachAction(e, entitySet.EntityContainer));
        }
        private Entity DetachAction(Entity entity, EntityContainer entityContainer)
        {
            var entitySetToDetachFrom = entityContainer.GetEntitySet(entity.GetType());
            entitySetToDetachFrom.Detach(entity);
            return entity;
        }
        #endregion
        #region RemoveEntityGraph
        /// <summary>
        /// Method that deletes a graph of associated entities from their respective entity sets.
        /// The graph of entities is defined by the 'EntityGraphAttribute' attribute.
        /// </summary>
        /// <param name="entitySet"></param>
        public void RemoveEntityGraph<TEntity>(EntitySet<TEntity> entitySet) where TEntity : Entity
        {
            GraphMap(e => RemoveAction(e, entitySet.EntityContainer));
        }
        private static Entity RemoveAction(Entity entity, EntityContainer entityContainer)
        {
            var entitySetToDetachFrom = entityContainer.GetEntitySet(entity.GetType());
            entitySetToDetachFrom.Remove(entity);
            return entity;
        }
        #endregion
    }
}
