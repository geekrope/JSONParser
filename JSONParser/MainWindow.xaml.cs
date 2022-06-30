﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace JSONParser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //"{ \"token1\": \"\\\"Token2\\\":\\\"{{\\\"A\\\":\\\"B\\\"}, \\\"C\\\": true]\\\"\"}"
            //"{\"X\": \"Y:Z\", dfhdfhhdfh  \"W\": \"N\"}"
            //"{\"web-app\": {\n  \"servlet\": [   \n    {\n      \"servlet-name\": \"cofaxCDS\",\n      \"servlet-class\": \"org.cofax.cds.CDSServlet\",\n      \"init-param\": {\n        \"configGlossary:installationAt\": \"Philadelphia, PA\",\n        \"configGlossary:adminEmail\": \"ksm@pobox.com\",\n        \"configGlossary:poweredBy\": \"Cofax\",\n        \"configGlossary:poweredByIcon\": \"/images/cofax.gif\",\n        \"configGlossary:staticPath\": \"/content/static\",\n        \"templateProcessorClass\": \"org.cofax.WysiwygTemplate\",\n        \"templateLoaderClass\": \"org.cofax.FilesTemplateLoader\",\n        \"templatePath\": \"templates\",\n        \"templateOverridePath\": \"\",\n        \"defaultListTemplate\": \"listTemplate.htm\",\n        \"defaultFileTemplate\": \"articleTemplate.htm\",\n        \"useJSP\": false,\n        \"jspListTemplate\": \"listTemplate.jsp\",\n        \"jspFileTemplate\": \"articleTemplate.jsp\",\n        \"cachePackageTagsTrack\": 200,\n        \"cachePackageTagsStore\": 200,\n        \"cachePackageTagsRefresh\": 60,\n        \"cacheTemplatesTrack\": 100,\n        \"cacheTemplatesStore\": 50,\n        \"cacheTemplatesRefresh\": 15,\n        \"cachePagesTrack\": 200,\n        \"cachePagesStore\": 100,\n        \"cachePagesRefresh\": 10,\n        \"cachePagesDirtyRead\": 10,\n        \"searchEngineListTemplate\": \"forSearchEnginesList.htm\",\n        \"searchEngineFileTemplate\": \"forSearchEngines.htm\",\n        \"searchEngineRobotsDb\": \"WEB-INF/robots.db\",\n        \"useDataStore\": true,\n        \"dataStoreClass\": \"org.cofax.SqlDataStore\",\n        \"redirectionClass\": \"org.cofax.SqlRedirection\",\n        \"dataStoreName\": \"cofax\",\n        \"dataStoreDriver\": \"com.microsoft.jdbc.sqlserver.SQLServerDriver\",\n        \"dataStoreUrl\": \"jdbc:microsoft:sqlserver://LOCALHOST:1433;DatabaseName=goon\",\n        \"dataStoreUser\": \"sa\",\n        \"dataStorePassword\": \"dataStoreTestQuery\",\n        \"dataStoreTestQuery\": \"SET NOCOUNT ON;select test=\'test\';\",\n        \"dataStoreLogFile\": \"/usr/local/tomcat/logs/datastore.log\",\n        \"dataStoreInitConns\": 10,\n        \"dataStoreMaxConns\": 100,\n        \"dataStoreConnUsageLimit\": 100,\n        \"dataStoreLogLevel\": \"debug\",\n        \"maxUrlLength\": 500}},\n    {\n      \"servlet-name\": \"cofaxEmail\",\n      \"servlet-class\": \"org.cofax.cds.EmailServlet\",\n      \"init-param\": {\n      \"mailHost\": \"mail1\",\n      \"mailHostOverride\": \"mail2\"}},\n    {\n      \"servlet-name\": \"cofaxAdmin\",\n      \"servlet-class\": \"org.cofax.cds.AdminServlet\"},\n \n    {\n      \"servlet-name\": \"fileServlet\",\n      \"servlet-class\": \"org.cofax.cds.FileServlet\"},\n    {\n      \"servlet-name\": \"cofaxTools\",\n      \"servlet-class\": \"org.cofax.cms.CofaxToolsServlet\",\n      \"init-param\": {\n        \"templatePath\": \"toolstemplates/\",\n        \"log\": 1,\n        \"logLocation\": \"/usr/local/tomcat/logs/CofaxTools.log\",\n        \"logMaxSize\": \"\",\n        \"dataLog\": 1,\n        \"dataLogLocation\": \"/usr/local/tomcat/logs/dataLog.log\",\n        \"dataLogMaxSize\": \"\",\n        \"removePageCache\": \"/content/admin/remove?cache=pages&id=\",\n        \"removeTemplateCache\": \"/content/admin/remove?cache=templates&id=\",\n        \"fileTransferFolder\": \"/usr/local/tomcat/webapps/content/fileTransferFolder\",\n        \"lookInContext\": 1,\n        \"adminGroupID\": 4,\n        \"betaServer\": true}}],\n  \"servlet-mapping\": {\n    \"cofaxCDS\": \"/\",\n    \"cofaxEmail\": \"/cofaxutil/aemail/*\",\n    \"cofaxAdmin\": \"/admin/*\",\n    \"fileServlet\": \"/static/*\",\n    \"cofaxTools\": \"/tools/*\"},\n \n  \"taglib\": {\n    \"taglib-uri\": \"cofax.tld\",\n    \"taglib-location\": \"/WEB-INF/tlds/cofax.tld\"}}}"
            //"{\"10\":\"\\\"a\\\":\\\"b\\\"\"}"
            //"{\n    \"$schema\": \"http://json-schema.org/draft-07/schema#\",\n    \"$id\": \"http://json-schema.org/draft-07/schema#\",\n    \"title\": \"Core schema meta-schema\",\n    \"definitions\": {\n        \"schemaArray\": {\n            \"type\": \"array\",\n            \"minItems\": 1,\n            \"items\": { \"$ref\": \"#\" }\n        },\n        \"nonNegativeInteger\": {\n            \"type\": \"integer\",\n            \"minimum\": 0\n        },\n        \"nonNegativeIntegerDefault0\": {\n            \"allOf\": [\n                { \"$ref\": \"#/definitions/nonNegativeInteger\" },\n                { \"default\": 0 }\n            ]\n        },\n        \"simpleTypes\": {\n            \"enum\": [\n                \"array\",\n                \"boolean\",\n                \"integer\",\n                \"null\",\n                \"number\",\n                \"object\",\n                \"string\"\n            ]\n        },\n        \"stringArray\": {\n            \"type\": \"array\",\n            \"items\": { \"type\": \"string\" },\n            \"uniqueItems\": true,\n            \"default\": []\n        }\n    },\n    \"type\": [\"object\", \"boolean\"],\n    \"properties\": {\n        \"$id\": {\n            \"type\": \"string\",\n            \"format\": \"uri-reference\"\n        },\n        \"$schema\": {\n            \"type\": \"string\",\n            \"format\": \"uri\"\n        },\n        \"$ref\": {\n            \"type\": \"string\",\n            \"format\": \"uri-reference\"\n        },\n        \"$comment\": {\n            \"type\": \"string\"\n        },\n        \"title\": {\n            \"type\": \"string\"\n        },\n        \"description\": {\n            \"type\": \"string\"\n        },\n        \"default\": true,\n        \"readOnly\": {\n            \"type\": \"boolean\",\n            \"default\": false\n        },\n        \"writeOnly\": {\n            \"type\": \"boolean\",\n            \"default\": false\n        },\n        \"examples\": {\n            \"type\": \"array\",\n            \"items\": true\n        },\n        \"multipleOf\": {\n            \"type\": \"number\",\n            \"exclusiveMinimum\": 0\n        },\n        \"maximum\": {\n            \"type\": \"number\"\n        },\n        \"exclusiveMaximum\": {\n            \"type\": \"number\"\n        },\n        \"minimum\": {\n            \"type\": \"number\"\n        },\n        \"exclusiveMinimum\": {\n            \"type\": \"number\"\n        },\n        \"maxLength\": { \"$ref\": \"#/definitions/nonNegativeInteger\" },\n        \"minLength\": { \"$ref\": \"#/definitions/nonNegativeIntegerDefault0\" },\n        \"pattern\": {\n            \"type\": \"string\",\n            \"format\": \"regex\"\n        },\n        \"additionalItems\": { \"$ref\": \"#\" },\n        \"items\": {\n            \"anyOf\": [\n                { \"$ref\": \"#\" },\n                { \"$ref\": \"#/definitions/schemaArray\" }\n            ],\n            \"default\": true\n        },\n        \"maxItems\": { \"$ref\": \"#/definitions/nonNegativeInteger\" },\n        \"minItems\": { \"$ref\": \"#/definitions/nonNegativeIntegerDefault0\" },\n        \"uniqueItems\": {\n            \"type\": \"boolean\",\n            \"default\": false\n        },\n        \"contains\": { \"$ref\": \"#\" },\n        \"maxProperties\": { \"$ref\": \"#/definitions/nonNegativeInteger\" },\n        \"minProperties\": { \"$ref\": \"#/definitions/nonNegativeIntegerDefault0\" },\n        \"required\": { \"$ref\": \"#/definitions/stringArray\" },\n        \"additionalProperties\": { \"$ref\": \"#\" },\n        \"definitions\": {\n            \"type\": \"object\",\n            \"additionalProperties\": { \"$ref\": \"#\" },\n            \"default\": {}\n        },\n        \"properties\": {\n            \"type\": \"object\",\n            \"additionalProperties\": { \"$ref\": \"#\" },\n            \"default\": {}\n        },\n        \"patternProperties\": {\n            \"type\": \"object\",\n            \"additionalProperties\": { \"$ref\": \"#\" },\n            \"propertyNames\": { \"format\": \"regex\" },\n            \"default\": {}\n        },\n        \"dependencies\": {\n            \"type\": \"object\",\n            \"additionalProperties\": {\n                \"anyOf\": [\n                    { \"$ref\": \"#\" },\n                    { \"$ref\": \"#/definitions/stringArray\" }\n                ]\n            }\n        },\n        \"propertyNames\": { \"$ref\": \"#\" },\n        \"const\": true,\n        \"enum\": {\n            \"type\": \"array\",\n            \"items\": true,\n            \"minItems\": 1,\n            \"uniqueItems\": true\n        },\n        \"type\": {\n            \"anyOf\": [\n                { \"$ref\": \"#/definitions/simpleTypes\" },\n                {\n                    \"type\": \"array\",\n                    \"items\": { \"$ref\": \"#/definitions/simpleTypes\" },\n                    \"minItems\": 1,\n                    \"uniqueItems\": true\n                }\n            ]\n        },\n        \"format\": { \"type\": \"string\" },\n        \"contentMediaType\": { \"type\": \"string\" },\n        \"contentEncoding\": { \"type\": \"string\" },\n        \"if\": { \"$ref\": \"#\" },\n        \"then\": { \"$ref\": \"#\" },\n        \"else\": { \"$ref\": \"#\" },\n        \"allOf\": { \"$ref\": \"#/definitions/schemaArray\" },\n        \"anyOf\": { \"$ref\": \"#/definitions/schemaArray\" },\n        \"oneOf\": { \"$ref\": \"#/definitions/schemaArray\" },\n        \"not\": { \"$ref\": \"#\" }\n    },\n    \"default\": true\n}"

            var parse = JSON.Parse("{\"d\":10,}");
            var toString = parse.ToJSON();
            var parse2 = JSON.Parse(toString);
        }
    }
}
