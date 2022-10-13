using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace TrakNusSparepartSystem.Workflow.Helper
{
    public class ParserAgent
    {
        /// <summary>
        /// Change the template for email with the parameter for the primary entity
        /// </summary>
        /// <param name="organizationService">CRM Organization</param>
        /// <param name="primaryEntity">Primary entity that parameter gonna take data from</param>
        /// <param name="pattern">String Regex Pattern</param>
        /// <returns></returns>
        public string SetParameterValue(IOrganizationService organizationService, Entity primaryEntity, String pattern)
        {
            //DECLARE HOW IT WORKS
            //Traversal Operator = .
            //Traversal Operator Requires The Before value to be EntityReference, hence if not entityreference
            //it will return only the value attribute before traversal operator (if exist, else it will return empty string
            //If the last value is DateTime it will be converted to localtime
            //ToString operator for format, ToString(format) operator need to be at the end
            //At the end of pattern requires it to be closed with |
            //If not closed with | then it will be appended to the pattern

            var regexPattern = @"(.+?)[.|]";

            Entity currentEntity = primaryEntity;
            object currentObject = String.Empty;
            String returnString = String.Empty;

            if (!pattern.EndsWith("|"))
            {
                pattern = pattern + "|";
            }

            var matches = Regex.Matches(pattern, regexPattern);

            foreach (Match match in matches)
            {
                var opr = match.Value[match.Value.Length - 1];
                var value = match.Value.Remove(match.Value.Length - 1);
                //If this requires format and is the end of attribute traversal
                if (opr == '|' && Regex.IsMatch(value, @"ToString\((.*)\)"))
                {
                    if (currentObject is DateTime)
                    {
                        currentObject = ((DateTime)currentObject).ToLocalTime();
                    }
                    else if (currentObject is Money)
                    {
                        currentObject = ((Money)currentObject).Value;
                    }
                    var regex = Regex.Match(value, @"\((.*)\)").Groups[1].Value.Replace("\"", "").Replace("'", ""); //TODO : check this replacing " and ' is okay
                    try
                    {

                        currentObject = currentObject.GetType()
                            .InvokeMember("ToString", BindingFlags.Public | BindingFlags.NonPublic |
                                                      BindingFlags.Instance | BindingFlags.InvokeMethod |
                                                      BindingFlags.IgnoreCase, null, currentObject, new object[] { regex })
                            as String;
                    }
                    catch (Exception ex)
                    {
                        currentObject = "";
                    }
                    break;
                }
                else // if this is the end then
                    if (opr == '|')
                    {
                        if (currentEntity.Contains(value))
                        {
                            currentObject = currentEntity[value];
                        }
                        else
                        {
                            currentObject = "";
                        }
                    }
                    else if (opr == '.') //if need to traverse(if entity reference only), else maybe there is format at end
                    {
                        if (currentEntity.Contains(value))
                        {
                            currentObject = currentEntity[value];
                            if (currentEntity[value] is EntityReference)
                            {
                                EntityReference reference = currentEntity.GetAttributeValue<EntityReference>(value);
                                currentEntity = organizationService.Retrieve(reference.LogicalName, reference.Id,
                                    new ColumnSet(true));
                            }
                        }
                    }
            }
            if (currentObject is EntityReference)
            {
                var reference = currentObject as EntityReference;
                currentObject = reference.Name;
            }
            else
                if (currentObject is DateTime)
                {
                    currentObject = ((DateTime)currentObject).ToLocalTime();
                }
                else if (currentObject is Money)
                {
                    currentObject = ((Money)currentObject).Value;
                }
                else if (currentObject is OptionSetValue)
                {
                    currentObject = ((OptionSetValue)currentObject).Value;
                }
            if (currentObject != null) returnString = currentObject.ToString();
            return returnString;
        }

        /// <summary>
        /// Parse Template, replace the {{attribute}} with attribute from primary entity
        /// </summary>
        /// <param name="input"></param>
        /// <param name="primaryEntity"></param>
        /// <param name="organizationService"></param>
        /// <returns></returns>
        public String ParseTemplate(String input, Entity primaryEntity, IOrganizationService organizationService)
        {
            string regexPattern = "{{(.+?)}}";
            try
            {
                var matches = Regex.Matches(input, regexPattern);

                List<String> stringList = new List<string>();
                foreach (Match match in matches)
                {
                    var attribute = match.Value.Replace("{{", "").Replace("}}", "");

                    var o = SetParameterValue(organizationService, primaryEntity, attribute);
                    input = input.Replace(match.Value, o.ToString());

                }
            }
            catch (Exception ex)
            {
                //suppress exception
            }
            return input;
        }
    }
}
