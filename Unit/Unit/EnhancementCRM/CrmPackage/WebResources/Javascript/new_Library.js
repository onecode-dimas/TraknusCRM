// Author: Cornel Croitoriu
// Web: www.Biz-Forward.com / www.Croitoriu.net
// E-mail: cornel@croitoriu.net
// Logical Operator
var LogicalOperator = { And:"And", Or:"Or", Like:"Like", Equal:"Equal", NotEqual:"NotEqual", GreaterThan:"GreaterThan", LessThan:"LessThan", Inner:"Inner", LeftOuter:"LeftOuter", Natural:"Natural", Eq:"eq", Null:"Null", NotNull:"NotNull" };
// CRM Service
function CrmService(entityName, logicalOperator) 
{
    if (logicalOperator == null)
        throw new Error("Must specify non-null value for logicalOperator");
    if (entityName == null)
        throw new Error("Must specify non-null value for entityName");
	
    this.entityName = entityName;
    this.ColumnSet = new Array();
    this.LogicalOperator = logicalOperator;
    this.Conditions = new Array();
    this.LinkedEntities = new Array();
}
// getEntityName
CrmService.prototype.getEntityName = function() 
{
    return this.entityName;
}
// Condition
function Condition(field, operator, value) 
{
    this.Field = field;
    this.Value = CrmEncodeDecode.CrmXmlEncode(value);
	
    if (operator == null)
        throw new Error("Must specify non-null value for operator");
	
    this.Operator = operator;
}
// setEntityName
CrmService.prototype.setEntityName = function() 
{
    return this.entityName;
}
// AddColumn
CrmService.prototype.AddColumn = function(columnName) 
{
    this.ColumnSet[this.ColumnSet.length] = columnName;
}
// AddFilterCondition
CrmService.prototype.AddFilterCondition = function(field, conditionOperator, value) 
{
    this.Conditions[this.Conditions.length] = new Condition(field, conditionOperator, value);
}
// Linked Entity
function LinkedEntity(linkFromEntityName, linkToEntityName, linkFromAttributeName, linkToAttributeName, joinOperator) 
{
    this.LinkFromEntityName = linkFromEntityName;
    this.LinkToEntityName = linkToEntityName;
    this.LinkFromAttributeName = linkFromAttributeName;
    this.LinkToAttributeName = linkToAttributeName;
    if (joinOperator == null)
        throw new Error("Must specify non-null value for operator");
    this.JoinOperator = joinOperator;
    this.Conditions = new Array();
    this.FilterOperator = LOGICAL_OPERATOR_AND;
}
// AddFilterCondition
LinkedEntity.prototype.AddFilterCondition = function(field, conditionOperator, value) 
{
    this.Conditions[this.Conditions.length] = new Condition(field, conditionOperator, value);
    return this.Conditions[this.Conditions.length - 1];
}
// AddLinkedEntityCondition
CrmService.prototype.AddLinkedEntityCondition = function(linkFromEntityName, linkToEntityName, linkFromAttributeName, linkToAttributeName, joinOperator) 
{
    this.LinkedEntities[this.LinkedEntities.length] = new LinkedEntity(linkFromEntityName, linkToEntityName, linkFromAttributeName, linkToAttributeName, joinOperator);
    return this.LinkedEntities[this.LinkedEntities.length - 1];
}
// Retrieve Multiple
function RetrieveMultipleResult(crmService) 
{
    this.Rows = new Array();
    this.CrmService = crmService;
}
// AddRow
RetrieveMultipleResult.prototype.AddRow = function() 
{
    this.Rows[this.Rows.length] = new Row();
    return this.Rows[this.Rows.length - 1];
}
 
// Row
function Row() 
{
    this.Columns = new Array();
}
// Column
function Column(columnName, value, dataType) 
{
    this.ColumnName = columnName;
    this.Value = value;
    this.DataType = dataType;
}
// AddColumn
Row.prototype.AddColumn = function(columnName, value) 
{
    this.Columns[this.Columns.length] = new Column(columnName, value);
}
// GetColumn
Row.prototype.GetColumn = function(columnName) 
{
    for (columnNumber in this.Columns) {
        var column = this.Columns[columnNumber];
        if (columnName.toLowerCase() == column.ColumnName.toLowerCase())
            return column;
    }
    throw new Error("Column " + columnName + " does not exist");
}
// GetValue
Row.prototype.GetValue = function(columnName) 
{
    var column = this.GetColumn(columnName);
    return column.Value;
}
// SOAP
CrmService.prototype.RetrieveMultiple = function()
{
    try {
        var xmlSoapHeader = "<?xml version=\"1.0\" encoding=\"utf-8\"?><soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">";
        var xmlAuthHeader = Xrm.Page.context.AuthenticationHeader();
        var xmlSoapBody = "<s:Body><Execute xmlns=\"http://schemas.microsoft.com/xrm/2011/Contracts/Services\" xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\"><request i:type=\"a:RetrieveMultipleRequest\" xmlns:a=\"http://schemas.microsoft.com/xrm/2011/Contracts\">";
        xmlSoapBody += "	<a:Parameters xmlns:b=\"http://schemas.datacontract.org/2004/07/System.Collections.Generic\">";
        xmlSoapBody += "		<a:KeyValuePairOfstringanyType>";
        xmlSoapBody += "    	<b:key>Query</b:key>";
        xmlSoapBody += "      <b:value i:type=\"a:QueryExpression\">";
		
        if(this.ColumnSet != null)
        {
            xmlSoapBody += "				<a:ColumnSet>";
            xmlSoapBody += "        <a:Columns xmlns:c=\"http://schemas.microsoft.com/2003/10/Serialization/Arrays\">";	
            if(this.ColumnSet.length != 1)
            {				
                xmlSoapBody += "					<a:AllColumns>false</a:AllColumns>";        			
				
                for (var i=0; i<this.ColumnSet.length; i++) {
                    var column = this.ColumnSet[i];
                    xmlSoapBody = xmlSoapBody + "<c:string>" + column + "</c:string>";
                }				
            }
            else 
                xmlSoapBody += "        	<a:AllColumns>true</a:AllColumns>";
            xmlSoapBody += "				</a:Columns>";
            xmlSoapBody += "        </a:ColumnSet>";
        }		
	    
        xmlSoapBody += "<a:Distinct>false</a:Distinct>";
        if (this.LinkedEntities.length > 0) {
            for (var i=0; i<this.LinkedEntities.length; i++) {
                var linkedEntity = this.LinkedEntities[linkedEntityNumber];
                xmlSoapBody += "<a:LinkEntities>";
                xmlSoapBody += "<a:LinkFromAttributeName>" + linkedEntity.LinkFromAttributeName + "</a:LinkFromAttributeName> ";
                xmlSoapBody += "<a:LinkFromEntityName>" + linkedEntity.LinkFromEntityName + "</a:LinkFromEntityName> ";
                xmlSoapBody += "<a:LinkToEntityName>" + linkedEntity.LinkToEntityName + "</a:LinkToEntityName> ";
                xmlSoapBody += "<a:LinkToAttributeName>" + linkedEntity.LinkToAttributeName + "</a:LinkToAttributeName> ";
                xmlSoapBody += "<a:JoinOperator>" + linkedEntity.JoinOperator + "</a:JoinOperator> ";
                xmlSoapBody += "<a:LinkCriteria> ";
                if (linkedEntity.FilterOperator == null)
                    throw new Error("Must specify non-null value for FilterOperator");
                xmlSoapBody += "<a:FilterOperator>" + linkedEntity.FilterOperator + "</a:FilterOperator> ";
                xmlSoapBody += "<a:Conditions> ";
                for (var i=0; i<linkedEntity.Conditions.length; i++) {
                    var conditionLinked = linkedEntity.Conditions[i];
                    xmlSoapBody += "<a:ConditionExpression> ";
                    xmlSoapBody += "<a:AttributeName>" + conditionLinked.Field + "</a:AttributeName> ";
                    xmlSoapBody += "<a:Operator>" + conditionLinked.Operator + "</a:Operator> ";
			    
                    if(conditionLinked.Operator != LogicalOperator.Null && conditionLinked.Operator != LogicalOperator.NotNull)
                    {
                        xmlSoapBody += "<a:Values> ";
                        xmlSoapBody += "<a:anyType i:type=\"d:string\" xmlns:d=\"http://www.w3.org/2001/XMLSchema\>" + conditionLinked.Value + "</a:anyType> ";
                        xmlSoapBody += "</a:Values> ";
                    }
			
                    xmlSoapBody += "</a:ConditionExpression> ";
                }
                xmlSoapBody += " </a:Conditions> ";
                xmlSoapBody += " <a:Filters /> ";
                xmlSoapBody += "</a:LinkCriteria> ";
                xmlSoapBody += "<a:LinkEntities />";
                xmlSoapBody += "</a:LinkEntity>";
            }
        }
        if (this.LogicalOperator == null)
            throw new Error("Must specify non-null value for LogicalOperator");
        xmlSoapBody += "</a:LinkEntities><a:Criteria><a:FilterOperator>" + this.LogicalOperator + "</a:FilterOperator><q1:Conditions>  "; 
        for (var i=0; i<this.Conditions.length; i++) {
            var condition = this.Conditions[i];
            if (condition.Operator == null)
                throw new Error("Must specify non-null value for condition Operator");
            xmlSoapBody += "<a:Condition><a:AttributeName>" + condition.Field + "</a:AttributeName><a:Operator>" + condition.Operator + "</a:Operator>";
		
            if(condition.Operator != LogicalOperator.Null && condition.Operator != LogicalOperator.NotNull)
                xmlSoapBody += "<a:Values><c:anyType i:type=\"d:string\" xmlns:d=\"http://www.w3.org/2001/XMLSchema\">" + condition.Value + "</c:Value></a:Values>";
            xmlSoapBody +="</a:Condition>";
        }
        xmlSoapBody += "</a:Conditions><a:Filters /></a:Criteria><a:Orders />";
        xmlSoapBody += "<a:NoLock>false</a:NoLock>";
        xmlSoapBody += "            </b:value>";
        xmlSoapBody += "          </a:KeyValuePairOfstringanyType>";
        xmlSoapBody += "        </a:Parameters>";
        xmlSoapBody += "        <a:RequestId i:nil=\"true\" />";
        xmlSoapBody += "        <a:RequestName>RetrieveMultiple</a:RequestName>";
        xmlSoapBody += "      </request>";
        xmlSoapBody += "    </Execute>";
        xmlSoapBody += "  </s:Body>";
        xmlSoapBody += "</s:Envelope>";
        var xmlt = xmlSoapHeader + xmlAuthHeader + xmlSoapBody;
        var xmlHttpRequest = new XMLHttpRequest();
        xmlHttpRequest.Open("POST", Xrm.Page.context.getServerUrl() + "/XRMServices/2011/Organization.svc/web", false);
        xmlHttpRequest.setRequestHeader("SOAPAction", "http://schemas.microsoft.com/xrm/2011/Contracts/Services/IOrganizationService/Execute");
        xmlHttpRequest.setRequestHeader("Content-Type", "text/xml; charset=utf-8");
        xmlHttpRequest.setRequestHeader("Accept", "application/xml, text/xml, */*");
	    
        var successCallback = null;
        var errorCallback = null;
        xmlHttpRequest.onreadystatechange = function () { RetrieveMultipleResponse(req, successCallback, errorCallback); };
        xmlHttpRequest.send(xmlt);
    
	    
        if (xmlHttpRequest.responseXML == null || xmlHttpRequest.responseXML.xml == null || xmlHttpRequest.responseXML.xml == "") {
            if (xmlHttpRequest.responseText != null && xmlHttpRequest.responseText != "")
                throw new Error(xmlHttpRequest.responseText);
            else
                throw new Error("Error returning response");
        }
        var xmlResponse = xmlHttpRequest.responseXML.xml;
        if (xmlHttpRequest.responseXML.documentElement.selectNodes("//error/description").length > 0) {
            throw new Error(xmlResponse);
        }
        var objNodeList = xmlHttpRequest.responseXML.documentElement.selectNodes("//BusinessEntity");
	    
        var totalNodesCount = objNodeList.length;
	    
        var result = new RetrieveMultipleResult(this);
        var nodeIndex = 0;
        var fieldTextTemp = "";
        var fieldText = "";
        if (totalNodesCount > 0) {
            do {
                var row = result.AddRow();
                for (var i=0; i<this.ColumnSet.length; i++) {
                    var columnName = this.ColumnSet[i];
                    fieldText = "";
                    var valueNode = objNodeList[nodeIndex].getElementsByTagName("q1:" + columnName)[0];
                    if (valueNode != null) {
                        fieldTextTemp = valueNode.childNodes[0].nodeValue;
                        if (fieldTextTemp != null && fieldTextTemp != "") {
                            fieldText = fieldText + fieldTextTemp;
                        }
                    }
                    row.AddColumn(columnName, fieldText);
                }
                nodeIndex = nodeIndex + 1;
            }
            while (totalNodesCount > nodeIndex)
        }
        return result;
    }
	catch(err) {

	}
	
return null;
}

//RetrieveMultipleResponse: function(req, successCallback, errorCallback) {
//    ///<summary>
//    /// Recieves the assign response
//    ///</summary>
//    ///<param name="req" Type="XMLHttpRequest">
//    /// The XMLHttpRequest response
//    ///</param>
//    ///<param name="successCallback" Type="Function">
//    /// The function to perform when an successfult response is returned.
//    /// For this message no data is returned so a success callback is not really necessary.
//    ///</param>
//    ///<param name="errorCallback" Type="Function">
//    /// The function to perform when an error is returned.
//    /// This function accepts a JScript error returned by the _getError function
//    ///</param>
               
               
//    if (req.readyState == 4) {
//        if (req.status == 200) {
//            //if (successCallback != null)
//            //***********************
//            //   ALERT RESULT HERE
//            //***********************
//            alert(req.responseXML.xml.toString()); }
//            //}
//        else {
//            errorCallback(SDK.SAMPLES._getError(req.responseXML));
//        }
//    }
//}

// Author: Cornel Croitoriu
// E-mail: cornel.croitoriu@novensys.com
// Make Struct
function MakeStruct(names) {
    try {
        var names = names.split(' ');
        var count = names.length;
		
        function constructor() 
        {
            for (var i = 0; i < count; i++)
                this[names[i]] = arguments[i];
        }
		
        return constructor;
    }
    catch(err)
    {
    }
}
// Global Structs
var FilterBy = MakeStruct("SchemaName Operator Value");
var ViewColumn = MakeStruct("SchemaName Width");
var CRMField = MakeStruct("SchemaName Value");
var MetadataObject = MakeStruct("SchemaName DisplayName");
// Advanced Filtered Lookup
function AdvancedFilteredLookup(lookupSchemaName, viewId, entityName, primaryKeyName, primaryFieldName, viewDisplayName, filterBy, orderBy, viewColumns) 
{
    var fetchXml = "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>" +
	"<entity name='" + entityName + "'>" +
	"<attribute name='" + primaryFieldName + "' />" +
	"<order attribute='" + orderBy + "' descending='false' />" +
	"<filter type='and'>" + 
	"<filter type='and'>";	
    for(var i=0; i< filterBy.length; i++)
        fetchXml += "<condition attribute='" + filterBy[i].SchemaName + "' operator='" + filterBy[i].Operator + "' value='" + filterBy[i].Value + "' />";	
    fetchXml += "</filter></filter></entity></fetch>";
	
    var layoutXml = "<grid name='resultset' " +
	"object='1' " +
	"jump='name' " +
	"select='1' " +
	"icon='1' " +
	"preview='1'>" +
	"<row name='result' " +
	"id='" + primaryKeyName + "'>";
    for(var i=0; i< viewColumns.length; i++)
        layoutXml += "<cell name='" + viewColumns[i].SchemaName + "' width='" + viewColumns[i].Width.toString() + "' />";
    layoutXml += "</row></grid>";
    try {		
        var lookupControl = Xrm.Page.ui.controls.get(lookupSchemaName);
        lookupControl.addCustomView(viewId, entityName, viewDisplayName, fetchXml, layoutXml, true);
    }
    catch(err) {
    }
}
// Sets the lookup value for a certain field
function SetLookupValue(fieldName, id, name, entityType) {
    if(fieldName != null) {
        try
        {
            var lookupValue = new Array();
            lookupValue[0] = new Object();
            lookupValue[0].id =  id;
            lookupValue[0].name = name;
            lookupValue[0].entityType  = entityType;
            Xrm.Page.getAttribute(fieldName).setValue(lookupValue);
        }
        catch(err)
        {
        }
    }
}
// Removes all the child nodes for a certain control
function removeChildNodes(ctrl)
{
    while (ctrl.childNodes[0])
    {
        ctrl.removeChild(ctrl.childNodes[0]);
    }
}
// Generate new Guid
function Hexa4()
{
    return (((1+Math.random())*0x10000)|0).toString(16).substring(1);
}
function GenerateGuid() 
{
    return (Hexa4()+Hexa4()+"-"+Hexa4()+"-"+Hexa4()+"-"+Hexa4()+"-"+Hexa4()+Hexa4()+Hexa4()).toUpperCase();
}
// Create Dynamic Button for CRM 4
/*function CreateButtonCRM4(fieldName, buttonText, buttonWidth, clickEvent)
{
	try
	{
		functiontocall=clickEvent;	
		crmForm.all.item(fieldName + "_c").style.display = "none";
		crmForm.all.item(fieldName).DataValue = buttonText;
		crmForm.all.item(fieldName).style.borderRight="#3366cc 1px solid";
		crmForm.all.item(fieldName).style.paddingRight="5px";
		crmForm.all.item(fieldName).style.borderTop="#3366cc 1px solid";
		crmForm.all.item(fieldName).style.paddingLeft="5px";
		crmForm.all.item(fieldName).style.fontSize="11px";
		crmForm.all.item(fieldName).style.backgroundImage="url(/_imgs/btn_rest.gif)";
		crmForm.all.item(fieldName).style.borderLeft="#3366cc 1px solid";
		crmForm.all.item(fieldName).style.width=buttonWidth;
		crmForm.all.item(fieldName).style.cursor="hand";
		crmForm.all.item(fieldName).style.lineHeight="18px";
		crmForm.all.item(fieldName).style.borderBottom="#3366cc 1px solid";
		crmForm.all.item(fieldName).style.backgroundRepeat="repeat-x";
		crmForm.all.item(fieldName).style.fontFamily="Tahoma";
		crmForm.all.item(fieldName).style.height="20px";
		crmForm.all.item(fieldName).style.backgroundColor="#cee7ff";
		crmForm.all.item(fieldName).style.textAlign="center";
		crmForm.all.item(fieldName).style.overflow="hidden";
		crmForm.all.item(fieldName).attachEvent("onmousedown",push_custom_button);
		crmForm.all.item(fieldName).attachEvent("onmouseup",release_custom_button);
		crmForm.all.item(fieldName).attachEvent("onclick",functiontocall);	
		crmForm.all.item(fieldName).contentEditable= false;
	}
	catch(err)
	{
	}
}
// Create Dynamic Button for CRM 5
function CreateButtonCRM5(fieldName, buttonText, buttonWidth, iconName, clickEvent)
{
	try
	{
		functiontocall=clickEvent;	
		//crmForm.all.item(fieldName + "_c").style.display = "none";
		Xrm.Page.ui crmForm.all.item(fieldName + "_c").style.display = "none";
		
		var li = document.createElement("LI");
		li.setAttribute('id', fieldName + 'LI');
		li.setAttribute('className', 'ms-crm-Menu');
		li.setAttribute('title', buttonText);
		li.setAttribute('onclick', functiontocall);
		li.setAttribute('onmousedown', push_custom_button);
		li.setAttribute('onmouseup', release_custom_button);	
		li.style.width=buttonWidth;
		li.style.cursor="hand";
		li.style.textAlign="center";
		li.style.overflow="hidden";
		
		var span = document.createElement("span");
		span.setAttribute('className', 'ms-crm-Menu-Label');
		span.setAttribute('id', fieldName + 'Span');
		span.style.cursor = "hand";
		li.appendChild(span);
		li.onmouseover = function() { span.setAttribute('className', 'ms-crm-Menu-Label-Hovered'); }
		li.onmouseout = function() { span.setAttribute('className', 'ms-crm-Menu-Label'); }
		
		var a = document.createElement("a");
		a.setAttribute('id', fieldName + 'A');
		a.setAttribute('className', 'ms-crm-Menu-Label');
		a.onclick = function() { return false; }
		a.setAttribute('target', '_self');
		a.setAttribute('href', 'javascript:onclick();');
		a.style.cursor = "hand";
		span.appendChild(a);
		
		var img = document.createElement("img");
		img.setAttribute('id', fieldName + 'Img');
		img.setAttribute('className', 'ms-crm-Menu-ButtonFirst');
		img.setAttribute('src', '/_imgs/ico/' + iconName);
		img.style.cursor = "hand";
		
		var span2 = document.createElement("span");
		span2.setAttribute('id', fieldName + 'Span2');
		span2.setAttribute('className', 'ms-crm-MenuItem-TextRTL');
		span2.innerText = buttonText;
		span2.style.cursor = "hand";
		a.appendChild(img);	
		a.appendChild(span2);
		
		removeChildNodes(crmForm.all.item(fieldName + "_d"));
		crmForm.all.item(fieldName + "_d").appendChild(li);
	}
	catch(err)
	{
	}
}*/
function push_custom_button()
{
    window.event.srcElement.style.marginLeft="1px";
    window.event.srcElement.style.marginTop="1px";
}
function release_custom_button()
{
    window.event.srcElement.style.marginLeft="0px";
    window.event.srcElement.style.marginTop="0px";
}
// Show Field
function ShowField(fieldName) 
{
    Xrm.Page.getControl(fieldName).setVisible(true); 
}
// Hide Field
function HideField(fieldName) 
{
    Xrm.Page.getControl(fieldName).setVisible(false); 
}
// Add days to a certain Date
Date.prototype.addDays = function(days) {
    this.setDate(this.getDate()+days);
} 
// Formats the date into a certain format
Date.prototype.Format = function(format)
{
    var d = this;
    var f = "";
	
    try {
        f = f + format.replace( /dd|mm|yyyy|MM|hh|ss|ms|APM|\s|\/|\-|,|\./ig , 
        function match()
        {
            switch(arguments[0])
            {
                case "dd": 
                    var dd = d.getDate();
                    return (dd < 10)? "0" + dd : dd;
                case "mm":
                    var mm = d.getMonth() + 1;
                    return (mm < 10)? "0" + mm : mm; 
                case "yyyy": return d.getFullYear();
                case "hh": 
                    var hh = d.getHours();
                    return (hh < 10)? "0" + hh : hh;
                case "MM": 
                    var MM = d.getMinutes(); 
                    return (MM < 10)? "0" + MM : MM;
                case "ss": 
                    var ss = d.getSeconds(); 
                    return (ss < 10)? "0" + ss : ss;
                case "ms": return d.getMilliseconds();
                case "APM": 
                    var apm = d.getHours(); 
                    return (apm < 12)? "AM" : "PM";
                default: return arguments[0];
            }
        });
    }
    catch(err)
    {
    }
    return f;
}
// Formats the date to CRM format
Date.prototype.toCRMFormat = function()
{
    var d = this;
    var f = d.Format("yyyy-mm-ddThh:MM:ss+" + (-d.getTimezoneOffset()/60) + ":00");
    return f;
}
// ********************************
// CREATE NEW ENTITY RECORD
// ********************************
// Call Crm Service

// Call Crm Service
function CallCrmService(soapBody, method, EntityName, Id) 
{	 
    try {
        var xmlHttpRequest = new XMLHttpRequest();
		 
        if (method == 'Create')
        {
            xmlHttpRequest.Open("POST", Xrm.Page.context.getServerUrl() + "/XRMServices/2011/Organization.svc/" +EntityName+ "Set", false); //synchronous, need to add EntityNameSet after webservice
        }
        else if (method == 'Update')
        {
            xmlHttpRequest.Open("POST", Xrm.Page.context.getServerUrl() + "/XRMServices/2011/Organization.svc/" +EntityName+ "Set(guid'" + Id +"')", false); //synchronous, need to add EntityNameSet after webservice
        }
		 		 
        xmlHttpRequest.setRequestHeader("Accept", "application/json");		 
        xmlHttpRequest.setRequestHeader("Content-Type", "text/xml; charset=utf-8");		 	 
		 
        if (method == 'Create')
        {
            xmlHttpRequest.onreadystatechange = function(){createCallBack(this)};
        }
        else if (method == 'Update')
        {
            xmlHttpRequest.onreadystatechange = function(){updateCallBack(this, Id)};
        }
        xmlHttpRequest.send(soapBody);
		 
        var resultXml = xmlHttpRequest.responseText.d;
        //var errorCount = resultXml.selectNodes('//error').length;
		
        //if (errorCount != 0) {
        //var msg = resultXml.selectSingleNode('//description').nodeTypedValue;
        //alert(msg);
			 
        //return null;
        //}
		 
        return resultXml;
    }
    catch(err) {
    }
	 
    return null;
}
 
function createCallBack(xmlHttpRequest) {
    if (xmlHttpRequest.readyState == 4 /* complete */) {
        if (xmlHttpRequest.status == 200) {
            //Success
            //var retrievedAccount = JSON.parse(xmlHttpRequest.responseText).d;
            //showMessage("ACTION: Retrieved account Name = \"" + retrievedAccount.Name + "\", AccountId = {" + retrievedAccount.AccountId + "}");

            //NEXT STEP: Update the account
            //updateAccountRecord(retrievedAccount.AccountId);
            //showMessage("xmlHttpRequestCallBack function success END");
        }
        else {
            //Failure
            errorHandler(xmlHttpRequest);
            showMessage("xmlHttpRequestCallBack function failure END");
        }
    }
}

function updateCallBack(xmlHttpRequest, Id) {
    if (xmlHttpRequest.readyState == 4 /* complete */) {
        //There appears to be an issue where IE maps the 204 status to 1223 when no content is returned.
        if (xmlHttpRequest.status == 204 || xmlHttpRequest.status == 1223) {
            //Success
            //showMessage("ACTION: Updated account data.");

            //NEXT STEP: Delete the account
            //deleteAccountRecord(Id);
            //showMessage("xmlHttpRequestCallBack function success END");
        }
        else {
            //Failure
            errorHandler(xmlHttpRequest);
            showMessage("updateAccountReqCallBack function failure END");
        }
    }
}
 
// Create Record
function CreateRecord(entityName, fields) 
{
    try {
        var resultArray = new Array();
        var attributesList = '';
		
        for( i = 0; i < fields.length; i++ ){
            if (fields[i].Value != null)
                attributesList += entityName+"."+fields[i].SchemaName+" = " + fields[i].Value + ";";
        }
		
        var xml = "var "+entityName+" = new Object()";
        xml += attributesList;
        xml += "var jsonAccount = window.JSON.stringify( + " + entityName + ");"; 
 				
        var resultXml = CallCrmService(xml, 'Create', entityName, null);
        if (resultXml) {
            //var newid = resultXml.selectSingleNode('//CreateResult').nodeTypedValue;
            //return newid;
        }
    }
    catch(err) {
    }
	 
    return null;
}

// Update Record
function UpdateRecord(entityName, entityId, fields) 
{
    try {
        if(entityId != null) {
            var resultArray = new Array();
            var attributesList = '';
			
            for( i = 0; i < fields.length; i++ ){
                if (fields[i].Value != null)
                    attributesList += entityName+"."+fields[i].SchemaName+" = " + fields[i].Value + ";";
            }
			
            var xml = "var "+entityName+" = new Object()";
            xml += attributesList;
            xml += "var jsonAccount = window.JSON.stringify( + " + entityName + ");"; 
						
            var resultXml = CallCrmService(xml, 'Update', entityName, entityId);
			
            return true;
        }
    }
    catch(err) {
    }
	 
    return false;
}

// Update All Records
function UpdateAllRecords(entityName, entityId, fields) 
{
    try {
        var resultArray = new Array();
        var attributesList = '';
		
        for( i = 0; i < fields.length; i++ ){
            if (fields[i].Value != null)
                attributesList += entityName+"."+fields[i].SchemaName+" = " + fields[i].Value + ";";
        }
		
        var xml = "var "+entityName+" = new Object()";
        xml += attributesList;
        xml += "var jsonAccount = window.JSON.stringify( + " + entityName + ");"; 
						
        var resultXml = CallCrmService(xml, 'Update', entityName, entityId);
		
        return true;
    }
    catch(err) {
    }
	 
    return false;
}

// Retrieve Multiple
function RetrieveRecords(entityName, outputColumns, filters) 
{
    try {
        // Service
        var crmService = new CrmService(entityName, LogicalOperator.And);
		
        // Columns
        crmService.AddColumn(entityName + "id");
        if(outputColumns != null)
        {
            for(var i=0; i< outputColumns.length; i++)
                crmService.AddColumn(outputColumns[i].SchemaName);
        }
        // Filters
        if(filters != null)
        {
            for(var i=0; i< filters.length; i++)
                crmService.AddFilterCondition(filters[i].SchemaName, filters[i].Operator, filters[i].Value);
        }
        // Retrieve
        var result = crmService.RetrieveMultiple();
		
        return result;
    }
    catch(err) {
    }
	
    return null;
}
// Retrieve Single
function RetrieveRecord(entityName, outputColumns, primaryKeyId) 
{
    try {
        var filters = [new FilterBy(entityName + 'id', LogicalOperator.Equal, primaryKeyId)];
		
        var items = RetrieveRecords(entityName, outputColumns, filters);
			
        if(items != null)
            return items.Rows[0];
    }
    catch(err) {
    }
	
    return null;
}
// Retrieve All Records of a certain entity
function RetrieveAllRecords(entityName, outputColumns) 
{
    try {
        // Service
        var crmService = new CrmService(entityName, LogicalOperator.And);
		
        // Columns
        crmService.AddColumn(entityName + "id");
        if(outputColumns != null)
        {
            for(var i=0; i< outputColumns.length; i++) {
                crmService.AddColumn(outputColumns[i].SchemaName);
            }
        }
        // Filters
        crmService.AddFilterCondition(entityName + "id", LogicalOperator.NotNull);
        // Retrieve
        var result = crmService.RetrieveMultiple();
        return result;
    }
    catch(err) {
    }
	
    return null;
}
// Create new Annotation
function CreateAnnotation(parentEntityName, parentEntityId, title, text)
{
    var fields = [new CRMField('objecttypecode', parentEntityName), new CRMField('objectid', parentEntityId), new CRMField('subject', title), new CRMField('notetext', text), new CRMField('isdocument', false), new CRMField("mimetype", "text/html")];
    return CreateRecord('annotation', fields);
}
// Gets the default currency
function GetDefaultCurrency()
{
    try {
        var entityName = 'transactioncurrency';
        var outputColumns = [new CRMField('transactioncurrencyid'), new CRMField('currencyname'), new CRMField('currencysymbol'), new CRMField('isocurrencycode')];
        var filters = [new FilterBy('isocurrencycode', LogicalOperator.Equal, 'RON')];
        var items = RetrieveRecords(entityName, outputColumns, filters);
        if(items != null)
            return items.Rows[0];
    }
    catch(err) {
    }
	
    return null;
}
// Prototypes
Array.prototype.Contains = function(obj) 
{    
    var i = this.length;    
    while (i--) {        
        if (this[i] == obj) 
            return true;        
    }    
	
    return false;
}
String.prototype.endsWith = function(str)
{    
    var lastIndex = this.lastIndexOf(str);    
    return (lastIndex != -1) && (lastIndex + str.length == this.length);
}
// Update Entity
function UpdateEntity(entityName, fieldName, value)
{
    var outputColumns = [new CRMField(fieldName)];
    var records = RetrieveAllRecords(entityName, outputColumns);
	
    if(records != null && records.Rows.length != 0 && entityName != null && fieldName != null && value != null)
    {
        for(var i=0; i<records.Rows.length; i++)
        {
            var entityId = records.Rows[i].GetValue(entityName+"id");
			
            if(entityId != null)
            {
                var fields = [new CRMField(fieldName, value)];
                UpdateAllRecords(entityName, entityId, fields);
            }
        }
    }
}
//*********************************************************
gQueryMetadataService = function(request) {
    /*
    Description:   Generic function to call the metadata webservice
    Calls by :  gGetEntityList() and gGetAttributeList()
    Returns:   XML response array
    Example:
      var request = "<Request xsi:type='RetrieveAllEntitiesRequest'>" +
        "<MetadataItems>EntitiesOnly</MetadataItems>" +  //MetadataItems ? The items you wish to retrieve e.g. EntitiesOnly, IncludeAttributes, IncludePrivileges, IncludeRelationships and All.
        "<RetrieveAsIfPublished>false</RetrieveAsIfPublished>" +  //RetrieveAsIfPublished ? specifies whether you want to get a list of just the published entity metadata or all of the entity metadata.
        "</Request>";
      var result = gQueryMetadataService(request);
      var logicalNames = result.selectNodes("//CrmMetadata/LogicalName");
    */
 
    var xmlhttp = new XMLHttpRequest();
    xmlhttp.open("POST", Xrm.Page.context.getServerUrl() + "/XRMServices/2011/Organization.svc/web", false);
    xmlHttpRequest.setRequestHeader("Accept", "application/xml, text/xml, */*");
    xmlhttp.setRequestHeader("Content-Type", "text/xml; charset=utf-8");
    xmlhttp.setRequestHeader("SOAPAction", 'http://schemas.microsoft.com/xrm/2011/Contracts/Services/IOrganizationService/Execute');
    var soapMessage = "<?xml version='1.0' encoding='utf-8'?>" +
    "<soap:Envelope xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' " +
    "xmlns:xsd='http://www.w3.org/2001/XMLSchema' xmlns:soap='http://schemas.xmlsoap.org/soap/envelope/'>" +
    "<soap:Header>" +
    "<CrmAuthenticationToken xmlns='http://schemas.microsoft.com/crm/2007/WebServices'>" +
    "<AuthenticationType xmlns='http://schemas.microsoft.com/crm/2007/CoreTypes'>" + AUTHENTICATION_TYPE + "</AuthenticationType>" +
    "<OrganizationName xmlns='http://schemas.microsoft.com/crm/2007/CoreTypes'>" + Xrm.Page.context.getOrgUniqueName() + "</OrganizationName>" +
    "<CallerId xmlns='http://schemas.microsoft.com/crm/2007/CoreTypes'>00000000-0000-0000-0000-000000000000</CallerId>" +
    "</CrmAuthenticationToken>" +
    "</soap:Header>" +
    "<soap:Body><Execute xmlns='http://schemas.microsoft.com/crm/2007/WebServices'>" + request+ "</Execute></soap:Body>" +
    "</soap:Envelope>";
    xmlhttp.send(soapMessage);
    return xmlhttp.responseXML;
}
// Get CRM Entities
GetEntityList = function() 
{ 
    var request = "<Request xsi:type='RetrieveAllEntitiesRequest'>" +
	"<MetadataItems>EntitiesOnly</MetadataItems>" +     //MetadataItems ? The items you wish to retrieve e.g. EntitiesOnly, IncludeAttributes, IncludePrivileges, IncludeRelationships and All.
	"<RetrieveAsIfPublished>false</RetrieveAsIfPublished>" +  //RetrieveAsIfPublished ? specifies whether you want to get a list of just the published entity metadata or all of the entity metadata.
	"</Request>";
	
    var resultArr = new Array();
    var j = 0;
    var result = gQueryMetadataService(request);
    var logicalNames = result.selectNodes("//CrmMetadata/LogicalName");
    var displayNames = result.selectNodes("//CrmMetadata/SchemaName");
    for (var i = 0; i < logicalNames.length; i++) 
    {
        try
        {
            //only show customizable entities
            if (result.selectNodes("//CrmMetadata/IsCustomizable")[i].text == 'true')
            {
                resultArr[j] = new MetadataObject(logicalNames[i].text, (displayNames[i]) ? displayNames[i].text : logicalNames[i].text);
                j++;
            }
        }
        catch(err)
        {
        }
    }
	
    return resultArr;
}
 
// Get CRM Attributes for a certain entity
GetAttributeList = function(entityName) 
{ 
    var request = "<Request xsi:type='RetrieveEntityRequest'>" +
	"<MetadataId>00000000-0000-0000-0000-000000000000</MetadataId>" +
	"<EntityItems>IncludeAttributes</EntityItems>" + //EntityItems ? The items we wish to retrieve e.g. IncludeAttributes, EntityOnly, IncludePrivileges, IncludeRelationships and All.
	"<LogicalName>" + entityName + "</LogicalName>" + //LogicalName ? Schema name of the entity we are retrieving.
	"<IsCustomizable>1</IsCustomizable>" + //LogicalName ? Schema name of the entity we are retrieving.
	"<RetrieveAsIfPublished>true</RetrieveAsIfPublished>" + //RetrieveAsIfPublished ? specifies whether you want to get a list of just the published entity metadata or all of the entity metadata.
	"</Request>";
    var resultArr = new Array();
    var j = 0;
    var result = gQueryMetadataService(request);
    var schemaNames = result.selectNodes("//EntityMetadata/Attributes/Attribute/SchemaName");
    var displayNames = result.selectNodes("//EntityMetadata/Attributes/Attribute/DisplayName/LocLabels/LocLabel/Label");
		
    for (var i = 0; i < schemaNames.length; i++) 
    {
        resultArr[j] = new MetadataObject(schemaNames[i].text.toLowerCase(), (displayNames[i]) ? displayNames[i].text : schemaNames[i].text.toLowerCase());
        j++;
    }
	
    return resultArr;
}
// Gets an entity by name
function GetEntityByName(entityList, entityName)
{
    if(entityList != null && entityName != null)
    {
        for(var i=0; i < entityList.length; i++)
        {
            if(entityList[i].SchemaName.toLowerCase() == entityName.toLowerCase())
                return entityList[i];
        }
    }
	
    return null;
}
// Gets an attribute by name
function GetAttributeByName(attributeList, attributeName)
{
    if(attributeList != null && attributeName != null)
    {
        for(var i=0; i < attributeList.length; i++)
        {
            if(attributeList[i].SchemaName.toLowerCase() == attributeName.toLowerCase())
                return attributeList[i];
        }
    }
	
    return null;
}
// Checks if the Logged User has a certain role
function UserHasRole(roleName)
{
    var serverUrl = Xrm.Page.context.getServerUrl();
    try {
        var oDataEndpointUrl = serverUrl + "/XRMServices/2011/OrganizationData.svc/";
        oDataEndpointUrl += "RoleSet?$top=1&$filter=Name eq '" + roleName + "'";
        var service = GetRequestObject();
		
        if (service != null)
        {
            service.open("GET", oDataEndpointUrl, false);
            service.setRequestHeader("X-Requested-Width", "XMLHttpRequest");
            service.setRequestHeader("Accept", "application/json, text/javascript, */*");
            service.send(null);
            //alert(service.responseText);
		    
            var requestResults = eval('(' + service.responseText + ')').d;
		    
            if (requestResults != null && requestResults.results.length == 1)
            {
                var role = requestResults.results[0]; 
                var id = role.RoleId;
                var currentUserRoles = Xrm.Page.context.getUserRoles();
			
                for (var i = 0; i < currentUserRoles.length; i++)
                {
                    var userRole = currentUserRoles[i];
                    if (GuidsAreEqual(userRole, id))
                    {
                        return true;
                    }
                }
            }
        }
    }
    catch(err)
    {
        alert(err);
    }
    return false;
}

function GetRequestObject()
{
    if (window.XMLHttpRequest)
    {
        return new window.XMLHttpRequest;
    }
    else
    {
        try
        {
            return new XMLHttpRequest();
        }
        catch (ex)
        {
            return null;
        }
    }
}
function GuidsAreEqual(guid1, guid2)
{
    var isEqual = false;
    if (guid1 == null || guid2 == null)
    {
        isEqual = false;
    }
    else
    {
        isEqual = guid1.replace(/[{}]/g, "").toLowerCase() == guid2.replace(/[{}]/g, "").toLowerCase();
    }
    return isEqual;
}
// Launches a Dialog Popup
function LaunchDialog(organizationName, dialogId, objectId)
{
    if(organizationName != null && dialogId != null && objectId != null)
    {
        var url = "http://" + window.location.host + "/" + organizationName + "/cs/dialog/rundialog.aspx?DialogId=%7b" + dialogId + "%7d&amp;EntityName=task&amp;ObjectId=" + objectId;
        window.open(url, "AutomaticDialog", "status=0,toolbar=0,scrollbars=0");
    }
}
//Custom
function doesControlHaveAttribute(control) {
    var controlType = control.getControlType();
    return controlType != "iframe" && controlType != "webresource" && controlType != "subgrid";
}
function disableFormFields(onOff) {
    Xrm.Page.ui.controls.forEach(function (control, index) {
        if (doesControlHaveAttribute(control)) {
            control.setDisabled(onOff);
        }
    });
}
function disableFields ()
{
    var attributes = Xrm.Page.data.entity.attributes.get();
    for (var i in attributes)
    {
        try
        {
            if (attributes[i].getName() != "new_managerapproval")
                Xrm.Page.getControl(attributes[i].getName()).setDisabled(true);
        }
        catch(exc)
        {
    		
        }
        finally
        {
            Xrm.Page.getControl("new_managerapproval").setDisabled(false);
            Xrm.Page.ui.tabs.get("manager_approval").setVisible(false);
        }
    }
}