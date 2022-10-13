using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace TrakNusSparepartSystem.Plugins.Helper
{
    /// <summary>
    /// Entity Attribute Mapping that is used for Mapping Attributes for CRM Entity.
    /// </summary>
    public class EntityAttributeMapping
    {
        /// <summary>
        /// Source Attribute Entity
        /// </summary>
        public string SourceAttribute { get; private set; }
        /// <summary>
        /// Target Attribute Entity
        /// </summary>
        public string TargetAttribute { get; private set; }

        /// <summary>
        /// Creates Entity Attribute Mapping Object with specified source and target attribute.
        /// </summary>
        /// <param name="sourceAttribute">Source Attribute Entity</param>
        /// <param name="targetAttribute">Target Attribute Entity</param>
        public EntityAttributeMapping(string sourceAttribute, string targetAttribute)
        {
            SourceAttribute = sourceAttribute;
            TargetAttribute = targetAttribute;
        }

        public override string ToString()
        {
            return string.Format("Source Attribute : {0}\r\nTarget Attribute: {1}\r\n", SourceAttribute, TargetAttribute);
        }
    }

    /// <summary>
    /// Entity Helper for repeated action performed.
    /// </summary>
    public static class EntityHelper
    {
        /// <summary>
        /// Sync Entity from specified entity source into specified entity target for one record.
        /// </summary>
        /// <param name="organizationService">CRM Organization Service</param>
        /// <param name="sourceEntityId">CRM Source Entity Identifier</param>
        /// <param name="sourceEntityName">CRM Source Entity Name</param>
        /// <param name="targetEntityId">CRM Target Entity Identifier</param>
        /// <param name="targetEntityName">CRM Target Entity Name</param>
        /// <param name="attributeMappings">Attribute Mappings, This is for determining what attributes gonna be copied.</param>
        /// <param name="additionalProcessingFunc">
        /// This is for additional processing if after copying those attributes from source to target entity, 
        /// then we can apply a function to perform specific action.
        /// We can add a function (Func) that have 2 parameters. First Parameter is Source Entity. Second Parameter is Target Entity.
        /// The function must return an Entity. This returned Entity from this function is that gonna be updated to specified target entity id.
        /// If this is not specified, then it will skip this step.
        /// </param>
        public static void SyncEntity(IOrganizationService organizationService, Guid sourceEntityId, string sourceEntityName, Guid targetEntityId,
            string targetEntityName, List<EntityAttributeMapping> attributeMappings, Func<Entity, Entity, Entity> additionalProcessingFunc = null)
        {
            try
            {
                var sourceEntity = organizationService.Retrieve(sourceEntityName, sourceEntityId, new ColumnSet(true));

                var targetEntityUpdate = new Entity(targetEntityName)
                {
                    Id = targetEntityId
                };

                foreach (var attributeMapping in attributeMappings)
                {
                    if (sourceEntity.Contains(attributeMapping.SourceAttribute))
                    {
                        targetEntityUpdate[attributeMapping.TargetAttribute] =
                            sourceEntity[attributeMapping.SourceAttribute];
                    }
                }

                if (additionalProcessingFunc != null)
                {
                    targetEntityUpdate = additionalProcessingFunc(sourceEntity, targetEntityUpdate);
                }

                organizationService.Update(targetEntityUpdate);
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format(
                    "Error occured when sync-ing entity. Parameters : sourceEntityId::{0} , sourceEntityName::{1}, targetEntityId::{2}, targetEntityName::{3}, attributeMappings::{4}.Technical Details:\r\n{5}",
                    sourceEntityId, sourceEntityName, targetEntityId, targetEntityName,
                    attributeMappings.Select(mapping => mapping.ToString()).Aggregate((curr, next) => curr + next),
                    ex.ToString()));
            }
        }

        /// <summary>
        /// This function is for copying entity data from source entity into new entity (can be different entity) with mappings provided.
        /// </summary>
        /// <param name="sourceEntity">Source Entity</param>
        /// <param name="targetEntity">Target Entity</param>
        /// <param name="attributeMappings">Attribute Mappings.</param>
        /// <returns>Entity with copied attributes. (Target Entity)</returns>
        public static Entity CopyEntityDataIntoAnotherEntity(Entity sourceEntity, Entity targetEntity, List<EntityAttributeMapping> attributeMappings)
        {
            try
            {

                foreach (var attributeMapping in attributeMappings)
                {
                    if (sourceEntity.Contains(attributeMapping.SourceAttribute))
                    {
                        targetEntity[attributeMapping.TargetAttribute] =
                            sourceEntity[attributeMapping.SourceAttribute];
                    }
                }

                return targetEntity;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(
                    "Error occured when copy-ing entity data into another entity (New). Parameters : sourceEntity::{0}, newEntityname::{1}, attributeMappings::{2}.Technical Details:\r\n{3}"
                    , sourceEntity.Id + sourceEntity.LogicalName, targetEntity.Id + targetEntity.LogicalName,
                    attributeMappings.Select(mapping => mapping.ToString()).Aggregate((curr, next) => curr + next),
                    ex.ToString()));
            }
        }

    }


}
