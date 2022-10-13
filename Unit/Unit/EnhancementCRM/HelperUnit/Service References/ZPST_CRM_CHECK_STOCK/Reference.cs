﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EnhancementCRM.HelperUnit.ZPST_CRM_CHECK_STOCK {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="urn:sap-com:document:sap:rfc:functions", ConfigurationName="ZPST_CRM_CHECK_STOCK.ZPST_FM_002_v7")]
    public interface ZPST_FM_002_v7 {
        
        // CODEGEN: Generating message contract since the operation ZPST_FM_002 is neither RPC nor document wrapped.
        [System.ServiceModel.OperationContractAttribute(Action="urn:sap-com:document:sap:rfc:functions:ZPST_FM_002_v7:ZPST_FM_002Request", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        EnhancementCRM.HelperUnit.ZPST_CRM_CHECK_STOCK.ZPST_FM_002Response1 ZPST_FM_002(EnhancementCRM.HelperUnit.ZPST_CRM_CHECK_STOCK.ZPST_FM_002Request request);
        
        [System.ServiceModel.OperationContractAttribute(Action="urn:sap-com:document:sap:rfc:functions:ZPST_FM_002_v7:ZPST_FM_002Request", ReplyAction="*")]
        System.Threading.Tasks.Task<EnhancementCRM.HelperUnit.ZPST_CRM_CHECK_STOCK.ZPST_FM_002Response1> ZPST_FM_002Async(EnhancementCRM.HelperUnit.ZPST_CRM_CHECK_STOCK.ZPST_FM_002Request request);
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.3190.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:sap-com:document:sap:rfc:functions")]
    public partial class ZPST_FM_002 : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string cSRF_TOKENField;
        
        private string mATERIALField;
        
        private string pLANTField;
        
        private string uOMField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        public string CSRF_TOKEN {
            get {
                return this.cSRF_TOKENField;
            }
            set {
                this.cSRF_TOKENField = value;
                this.RaisePropertyChanged("CSRF_TOKEN");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=1)]
        public string MATERIAL {
            get {
                return this.mATERIALField;
            }
            set {
                this.mATERIALField = value;
                this.RaisePropertyChanged("MATERIAL");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=2)]
        public string PLANT {
            get {
                return this.pLANTField;
            }
            set {
                this.pLANTField = value;
                this.RaisePropertyChanged("PLANT");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=3)]
        public string UOM {
            get {
                return this.uOMField;
            }
            set {
                this.uOMField = value;
                this.RaisePropertyChanged("UOM");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.3190.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="urn:sap-com:document:sap:rfc:functions")]
    public partial class ZPSTMMS003 : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string wERKSField;
        
        private string mATNRField;
        
        private string uNITField;
        
        private decimal wKBSTField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        public string WERKS {
            get {
                return this.wERKSField;
            }
            set {
                this.wERKSField = value;
                this.RaisePropertyChanged("WERKS");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=1)]
        public string MATNR {
            get {
                return this.mATNRField;
            }
            set {
                this.mATNRField = value;
                this.RaisePropertyChanged("MATNR");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=2)]
        public string UNIT {
            get {
                return this.uNITField;
            }
            set {
                this.uNITField = value;
                this.RaisePropertyChanged("UNIT");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=3)]
        public decimal WKBST {
            get {
                return this.wKBSTField;
            }
            set {
                this.wKBSTField = value;
                this.RaisePropertyChanged("WKBST");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.3190.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="urn:sap-com:document:sap:rfc:functions")]
    public partial class ZPST_FM_002Response : object, System.ComponentModel.INotifyPropertyChanged {
        
        private ZPSTMMS003[] z_VALUESField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        [System.Xml.Serialization.XmlArrayItemAttribute("item", Form=System.Xml.Schema.XmlSchemaForm.Unqualified, IsNullable=false)]
        public ZPSTMMS003[] Z_VALUES {
            get {
                return this.z_VALUESField;
            }
            set {
                this.z_VALUESField = value;
                this.RaisePropertyChanged("Z_VALUES");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class ZPST_FM_002Request {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:sap-com:document:sap:rfc:functions", Order=0)]
        public EnhancementCRM.HelperUnit.ZPST_CRM_CHECK_STOCK.ZPST_FM_002 ZPST_FM_002;
        
        public ZPST_FM_002Request() {
        }
        
        public ZPST_FM_002Request(EnhancementCRM.HelperUnit.ZPST_CRM_CHECK_STOCK.ZPST_FM_002 ZPST_FM_002) {
            this.ZPST_FM_002 = ZPST_FM_002;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class ZPST_FM_002Response1 {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="urn:sap-com:document:sap:rfc:functions", Order=0)]
        public EnhancementCRM.HelperUnit.ZPST_CRM_CHECK_STOCK.ZPST_FM_002Response ZPST_FM_002Response;
        
        public ZPST_FM_002Response1() {
        }
        
        public ZPST_FM_002Response1(EnhancementCRM.HelperUnit.ZPST_CRM_CHECK_STOCK.ZPST_FM_002Response ZPST_FM_002Response) {
            this.ZPST_FM_002Response = ZPST_FM_002Response;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ZPST_FM_002_v7Channel : EnhancementCRM.HelperUnit.ZPST_CRM_CHECK_STOCK.ZPST_FM_002_v7, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ZPST_FM_002_v7Client : System.ServiceModel.ClientBase<EnhancementCRM.HelperUnit.ZPST_CRM_CHECK_STOCK.ZPST_FM_002_v7>, EnhancementCRM.HelperUnit.ZPST_CRM_CHECK_STOCK.ZPST_FM_002_v7 {
        
        public ZPST_FM_002_v7Client() {
        }
        
        public ZPST_FM_002_v7Client(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ZPST_FM_002_v7Client(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ZPST_FM_002_v7Client(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ZPST_FM_002_v7Client(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        EnhancementCRM.HelperUnit.ZPST_CRM_CHECK_STOCK.ZPST_FM_002Response1 EnhancementCRM.HelperUnit.ZPST_CRM_CHECK_STOCK.ZPST_FM_002_v7.ZPST_FM_002(EnhancementCRM.HelperUnit.ZPST_CRM_CHECK_STOCK.ZPST_FM_002Request request) {
            return base.Channel.ZPST_FM_002(request);
        }
        
        public EnhancementCRM.HelperUnit.ZPST_CRM_CHECK_STOCK.ZPST_FM_002Response ZPST_FM_002(EnhancementCRM.HelperUnit.ZPST_CRM_CHECK_STOCK.ZPST_FM_002 ZPST_FM_0021) {
            EnhancementCRM.HelperUnit.ZPST_CRM_CHECK_STOCK.ZPST_FM_002Request inValue = new EnhancementCRM.HelperUnit.ZPST_CRM_CHECK_STOCK.ZPST_FM_002Request();
            inValue.ZPST_FM_002 = ZPST_FM_0021;
            EnhancementCRM.HelperUnit.ZPST_CRM_CHECK_STOCK.ZPST_FM_002Response1 retVal = ((EnhancementCRM.HelperUnit.ZPST_CRM_CHECK_STOCK.ZPST_FM_002_v7)(this)).ZPST_FM_002(inValue);
            return retVal.ZPST_FM_002Response;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<EnhancementCRM.HelperUnit.ZPST_CRM_CHECK_STOCK.ZPST_FM_002Response1> EnhancementCRM.HelperUnit.ZPST_CRM_CHECK_STOCK.ZPST_FM_002_v7.ZPST_FM_002Async(EnhancementCRM.HelperUnit.ZPST_CRM_CHECK_STOCK.ZPST_FM_002Request request) {
            return base.Channel.ZPST_FM_002Async(request);
        }
        
        public System.Threading.Tasks.Task<EnhancementCRM.HelperUnit.ZPST_CRM_CHECK_STOCK.ZPST_FM_002Response1> ZPST_FM_002Async(EnhancementCRM.HelperUnit.ZPST_CRM_CHECK_STOCK.ZPST_FM_002 ZPST_FM_002) {
            EnhancementCRM.HelperUnit.ZPST_CRM_CHECK_STOCK.ZPST_FM_002Request inValue = new EnhancementCRM.HelperUnit.ZPST_CRM_CHECK_STOCK.ZPST_FM_002Request();
            inValue.ZPST_FM_002 = ZPST_FM_002;
            return ((EnhancementCRM.HelperUnit.ZPST_CRM_CHECK_STOCK.ZPST_FM_002_v7)(this)).ZPST_FM_002Async(inValue);
        }
    }
}