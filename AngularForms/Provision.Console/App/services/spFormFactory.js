"use strict";
app.factory('spFormFactory', function($location, $http, $log, $q, $rootScope) {

    var spFormFactory = {};
    spFormFactory.loading = false;

    var search = $location.search();
    spFormFactory.itemId = $location.search().ID;
    spFormFactory.userId = _spPageContextInfo.userId;
    spFormFactory.userLoginName = _spPageContextInfo.userLoginName;
    spFormFactory.isSiteAdmin = _spPageContextInfo.isSiteAdmin;

    if (window.location.href.indexOf('DispForm.aspx') != -1) {
        spFormFactory.formMode = 'View';
    } else if (window.location.href.indexOf('NewForm.aspx') != -1) {
        spFormFactory.formMode = 'New';
    } else {
        spFormFactory.formMode = 'Edit';
    }

    //INITIALIZATION START
    spFormFactory.initialize = function(scope, listTitle) {
        spFormFactory.scope = scope;
        scope.ListTitle = spFormFactory.ListTitle = listTitle;
        scope.formMode = spFormFactory.formMode;
        scope.save = spFormFactory.save;
        scope.cancel = spFormFactory.cancel;

        //OOTB SharePoint Save button ovverride: 
        window.PreSaveAction = function() {
            scope.save();
            scope.$apply();
        };

        spFormFactory.loading = true;
        return getFields()
            .then(initializeFields)
            .then(getFieldValues)
            .then(initializeFieldValues)
            .then(passResultsToScope)
            .then(getListEntityFullName)
            .then(setListEntityFullName)
            .finally(function(){
                spFormFactory.loading = false;
            });
    }
    
    function getFields() {
        spFormFactory.loading = true;
        var restQueryUrl = _spPageContextInfo.webServerRelativeUrl +
            "/_api/web/lists/getbytitle('" + spFormFactory.ListTitle + "')/Fields";
        return $http.get(restQueryUrl);
    };

    function initializeField(result) {
        var retVal = {};
        retVal.Id = result.Id;
        retVal.FieldDisplayName = result.Title;
        retVal.FieldInternalName = result.InternalName;
        retVal.FieldType = result.TypeAsString;
        retVal.Required = result.Required;
        retVal.ReadOnlyField = result.ReadOnlyField;
        if (result.Choices) {
            retVal.Choices = result.Choices.results;
        }

        return retVal;
    };

    function initializeFields(response) {
        var results = response.data.d.results;
        spFormFactory.f = {};
        for (var x = 0; x < results.length; x++) {
          //  if (!results[x].ReadOnlyField) {
                if (!results[x].Hidden) {
                    if (results[x].InternalName != 'ContentType') {
                        if (results[x].InternalName != 'Attachments') {
                            var field = initializeField(results[x]);
                            spFormFactory.f[results[x].InternalName] = field;                
                        }
                    }
                }
           // }
        }

        return response;
    }

    function getFieldValues() {
        if (spFormFactory.formMode == 'New') {
            return ;
        }
        var restQueryUrl = _spPageContextInfo.webServerRelativeUrl +
            "/_api/web/lists/getbytitle('" + spFormFactory.ListTitle + "')/Items(" + spFormFactory.itemId + ")";
        return $http.get(restQueryUrl);
    };

    function initializeFieldValues(response) {
        if (spFormFactory.formMode == 'New') {
            return;
        }

        //Saving the eTag so that we can reuse it when updating the items
        spFormFactory.etag = response.data.d.__metadata.etag

        var fields = response.data.d;
        spFormFactory.fieldValues = [];

        for (var field in spFormFactory.f) {
            var fieldValue = fields[spFormFactory.f[field].FieldInternalName];
            if (spFormFactory.f[field].FieldType == 'User') {
                //All user fields have 'Id' in their Names:
                fieldValue = fieldValue || fields[spFormFactory.f[field].FieldInternalName + 'Id']; 
                spFormFactory.f[field].Value = fieldValue;

                if (spFormFactory.f[field].Value != null) {
                    var getUserRestURL = _spPageContextInfo.webServerRelativeUrl + "/_api/Web/GetUserById(" + spFormFactory.f[field].Value + ")";

                    (function(currentFieldName) {
                        $http.get(getUserRestURL).then(function(requestResponse) {
                            spFormFactory.f[currentFieldName].user = {};
                            spFormFactory.f[currentFieldName].user.Id = requestResponse.data.d.Id;
                            spFormFactory.f[currentFieldName].user.Name = requestResponse.data.d.LoginName;
                            spFormFactory.f[currentFieldName].user.Title = requestResponse.data.d.Title;
                        });
                    })(field);
                }
            }
            if (spFormFactory.f[field].FieldType == 'DateTime') {
                if(fields[spFormFactory.f[field].FieldInternalName] != null)
                {
                    var fieldValue = moment(fields[spFormFactory.f[field].FieldInternalName]).toDate();
                }
                else {
                    fieldValue = null;
                }
            }
            spFormFactory.f[field].Value = fieldValue;
        }
    }

    function passResultsToScope() {
        spFormFactory.scope.f = spFormFactory.f;
    }

    function getListEntityFullName(){
        var restQueryUrl = _spPageContextInfo.webServerRelativeUrl +
            "/_api/web/lists/getbytitle('" + spFormFactory.ListTitle + "')/ListItemEntityTypeFullName";
             return $http.get(restQueryUrl);
    }

    function setListEntityFullName(promise){
        spFormFactory.ListItemEntityTypeFullName = promise.data.d.ListItemEntityTypeFullName;       
    }
    //INITIALIZATION END

    spFormFactory.broadcastSaveEventAndWaitForSubscribersToFinish = function(promise) {
        // Required to support multiple uploader controls on the same form
        $rootScope.$broadcast('spFormFactory:itemSaved', promise);

        // Track all child directive's post-save promises and ensure all are resolved before next step in promise chain
        return promise.then(function onSuccess() {
            return $q.all(postsavepromises); 
        } );
    }

    spFormFactory.save = function() {
        spFormFactory.loading = true;
        var requestUri = '';
        var header = {};
        if (spFormFactory.formMode == 'New') {
            requestUri = _spPageContextInfo.webServerRelativeUrl + "/_api/web/lists/GetByTitle('" + spFormFactory.ListTitle + "')/items";

            header = {
                "accept": "application/json;odata=verbose",
                "content-type": "application/json;odata=verbose",
                "X-RequestDigest": $("#__REQUESTDIGEST").val()
            }
        } else {
            requestUri = _spPageContextInfo.webServerRelativeUrl + "/_api/web/lists/GetByTitle('" + spFormFactory.ListTitle + "')/getItemById(" + spFormFactory.itemId + ")";
            header = {
                "accept": "application/json;odata=verbose",
                "content-type": "application/json;odata=verbose",
                "X-RequestDigest": $("#__REQUESTDIGEST").val(),
                "X-HTTP-Method": "MERGE", // needed for update
                "IF-MATCH": spFormFactory.etag
            }
        }

        $log.debug(getLatestFieldValues());
        var promise = $http({
            url: requestUri,
            method: "POST",
            data: getLatestFieldValues(),
            headers: header,
        }).then(onSuccess, onError);
       
        promise.then(spFormFactory.broadcastSaveEventAndWaitForSubscribersToFinish.bind(null, promise)).then(spFormFactory.cancel);

        function onSuccess(responseData) {
            if (spFormFactory.formMode == 'New') {
                // Read back the ID of the newly-created item so it can be used in a post-save action, if necessary
                if (responseData && responseData.data && responseData.data.d && responseData.data.d.ID){
                    spFormFactory.itemId = responseData.data.d.ID;
                } 

                SP.UI.Notify.addNotification("Item created.\nDon't close the form", false);
                
            } else {
                SP.UI.Notify.addNotification("Item updated.\nDon't close the form", false);
            }         
        }

        function onError(error) {
            console.log(error);
            if(error.statusText == 'Precondition Failed'){
                if(error.data.error.message.value.indexOf('ETag') != -1){
                    alert('Your changes conflict with those made concurrently by another user. If you want your changes to be applied, click Back in your Web browser, refresh the page, and resubmit your changes.');
                }
                else{
                    alert(JSON.stringify(error));
                }
            }
            else{
                alert(JSON.stringify(error));
            }
        }

        function getLatestFieldValues() {

            var fieldValues = '';
            for (var field in spFormFactory.f) {
                if(field.ReadOnlyField && field.ReadOnlyField == true){
                    continue;
                }
                if (spFormFactory.f[field].FieldInternalName != 'ContentType') {
                    if (spFormFactory.f[field].FieldType == 'User') {
                        if (angular.isUndefined(spFormFactory.f[field].user) == false) {
                            if (spFormFactory.f[field].user != null) {
                                fieldValues = fieldValues +
                                    "'" + spFormFactory.f[field].FieldInternalName + "Id': " +
                                    "'" + spFormFactory.f[field].user.Id + "',";
                            } else {
                                fieldValues = fieldValues +
                                    "'" + spFormFactory.f[field].FieldInternalName + "Id': " +
                                    "null,";
                            }
                        }

                    } else if (spFormFactory.f[field].FieldType == 'DateTime') {
                        if (angular.isUndefined(spFormFactory.f[field].Value) == false) {
                            var dateTime = moment(spFormFactory.f[field].Value);
                            var dateTimeISO = dateTime.toISOString(); //SharePoint only understands ISO date format
                            if (dateTimeISO.toString().toLowerCase().indexOf('invalid') == -1) {
                                fieldValues = fieldValues +
                                    "'" + spFormFactory.f[field].FieldInternalName + "': " +
                                    "'" + dateTimeISO + "',";
                            }
                        }
                    }
                    else {
                        //Regular fields like Text, Note, Choice, Number:
                        if (angular.isUndefined(spFormFactory.f[field].Value) == false) {
                            if (spFormFactory.f[field].Value != null) {
                                
                                if (spFormFactory.f[field].FieldType == 'Text') {
                                    if (spFormFactory.f[field].Value.toString().length > 255){
                                        spFormFactory.f[field].Value = spFormFactory.f[field].Value.toString().substring(0, 254);
                                    }
                                }

                                fieldValues = fieldValues +
                                    "'" + spFormFactory.f[field].FieldInternalName + "': " +
                                    "'" + spFormFactory.f[field].Value.toString().replace(/\\/g, '\\\\').replace(/\'/g, "\\'") + "',";
                            } else {
                                fieldValues = fieldValues +
                                    "'" + spFormFactory.f[field].FieldInternalName + "': " +
                                    "null,";
                            }
                        }
                    }
                }
            }
            var fieldValues2 = fieldValues.substring(0, fieldValues.length - 1);

            return "{'__metadata': " + JSON.stringify({
                'type': spFormFactory.ListItemEntityTypeFullName
            }) + (typeof(fieldValues2) === "string" && fieldValues2.length > 0 ? "," + fieldValues2 : "") + "}";
        }
    }

    spFormFactory.cancel = function() {
        if(window.frameElement){
            window.frameElement.cancelPopUp();
        }
        var Source = $location.search().Source;
        if (Source){
            window.location = Source;
        }
        else window.location = $('#DeltaPlaceHolderPageTitleInTitleArea a').first().attr('href');

    }

    spFormFactory.getFileEndpointUri = function(serverUrl, doclibname, foldername) {
        if (typeof(serverUrl) !== "string" || serverUrl.length <= 0) return undefined;

        var serverRelativeUrlToFolder = _spPageContextInfo.webServerRelativeUrl + 
            (typeof(doclibname) === "string" && doclibname.length > 0 ? '/' + doclibname : '') +
            (typeof(foldername) === "string" && foldername.length > 0 ? '/' + foldername : '');

        // Construct the endpoint.
        return String.format(
                "{0}/_api/web/getfolderbyserverrelativeurl('{1}')/files",
                serverUrl, serverRelativeUrlToFolder);
    }

    spFormFactory.loadAttachments = function(doclibname, foldername, filterfieldname, filterfieldvalue) {
        if (spFormFactory.formMode == "New") return null;

        var folderUsed = (typeof(foldername) === "string" && foldername.length > 0);

        var loadAttachmentsUrl = _spPageContextInfo.webServerRelativeUrl 
            + "/_api/web/lists/getbytitle('" + doclibname + "')/items"
            + "?$select=File&$expand=File"
            + (folderUsed ? "&$filter=" + filterfieldname + "%20eq%20" + filterfieldvalue : '');

        // Place attachments on page
        return $http.get(loadAttachmentsUrl).then(function(requestResponse) {
            var attachmentList = requestResponse.data.d.results;

            // Remove files that are in a different folder than the one used by this instance of the control
            if (folderUsed) {
                var folderUrlMatch = decodeURIComponent(_spPageContextInfo.webServerRelativeUrl + "/" + doclibname + "/" + foldername).toLowerCase();
                //attachmentList = attachmentList.filter(item => decodeURIComponent(item.File.ServerRelativeUrl).toLowerCase().startsWith(folderUrlMatch));
                attachmentList = attachmentList.filter(function(item) { return decodeURIComponent(item.File.ServerRelativeUrl).toLowerCase().startsWith(folderUrlMatch); });
            }
            
            //console.log(attachmentList);
            return attachmentList;

        }, onError);

        // Display error messages. 
        function onError(error) {
            console.log(error);
            alert(JSON.stringify(error));
        }
    }

    // Upload the file.
    // You can upload files up to 2 GB with the REST API.
    spFormFactory.uploadFileToDocLib = function (arrayBuffer, doclibname, foldername, fileuniquename, filedisplayname, customfilemetadata) {
        if (typeof(arrayBuffer) !== "object" || arrayBuffer === null) return;

        // Define the folder path for this example.
        var serverRelativeUrlToFolder = _spPageContextInfo.webServerRelativeUrl + '/' + doclibname +
            (typeof(foldername) === "string" && foldername.length > 0 ? '/' + foldername : '');

            return addFileToFolder(arrayBuffer).then(function successCallback(response) {
                return getListItem(response.d.ListItemAllFields.__deferred.uri).then(function successCallback(response) {
                    // Change the display name and title of the list item.
                    return updateListItem(response.data.d.__metadata, customfilemetadata).then(function successCallback(response) {
                        //console.log("File uploaded and updated" + (response && response.config && response.config.url ? ": " + response.config.url : ''));
                    }, onError);
              }, onError);
            }, onError);
        //}, onError);

        // Add the file to the file collection in the Shared Documents folder.

    function addFileToFolder(arrayBuffer) {
            // Construct the endpoint.
            var fileCollectionEndpoint = String.format(spFormFactory.getFileEndpointUri(_spPageContextInfo.webAbsoluteUrl, doclibname, foldername) + "/add(overwrite=true, url='{0}')", fileuniquename);

            // Send the request and return the response.
            // This call returns the SharePoint file.

            return jQuery.ajax({
                        url: fileCollectionEndpoint,
                        type: "POST",
                        data: arrayBuffer,
                        processData: false,
                        headers: {
                            "accept": "application/json;odata=verbose",
                            "X-RequestDigest": $("#__REQUESTDIGEST").val()
                            //,"content-length": arrayBuffer.byteLength
                        }
                    });
        }


        // Get the list item that corresponds to the file by calling the file's ListItemAllFields property.
        function getListItem(fileListItemUri) {
            // Send the request and return the response.
            return $http.get(fileListItemUri, {
                headers: { "accept": "application/json;odata=verbose" }
            });
        }

        // Change the display name and title of the list item.
        function updateListItem(itemMetadata, customFileMetadata) {
            // Define the list item changes. Use the FileLeafRef property to change the display name. 
            // Assemble the file update metadata 
            var metadata = {
                __metadata: {
                    type: itemMetadata.type
                },
                FileLeafRef: fileuniquename,
                Title: filedisplayname
            };

            // Add the custom metadata fields for this file to the metadata object
            if (typeof(customFileMetadata) === "object") $(metadata).extend(metadata, customFileMetadata);
            
            // Send the request and return the promise.
            // This call does not return response content from the server.
            var body = JSON.stringify(metadata);
            return $http.post(itemMetadata.uri, body, {
                headers: {
                    "X-RequestDigest": $("#__REQUESTDIGEST").val(),
                    "content-type": "application/json;odata=verbose",
                    "content-length": body.length,
                    "IF-MATCH": "*",
                    "X-HTTP-Method": "MERGE"
                }
            });
        }

        // Display error messages. 
        function onError(error) {
            console.log(error);
            alert(JSON.stringify(error));
        }
    }

    // Get the local file as an array buffer.
    spFormFactory.getFileBuffer = function (fileInput) {
      
        var deferred = $q.defer();
        var reader = new FileReader();
        reader.onloadend = function (e) {
            deferred.resolve(e.target.result);
        }
        reader.onerror = function (e) {
            deferred.reject(e.target.error);
        }
        reader.readAsArrayBuffer(fileInput.files[0]);
        return deferred.promise;
    }
    

    spFormFactory.deleteFileFromDocLib = function(doclibname, foldername, fileuniquename) {
        var fileCollectionEndpoint = String.format(spFormFactory.getFileEndpointUri(_spPageContextInfo.webAbsoluteUrl, doclibname, foldername) + "('{0}')", fileuniquename);

        return $http.post(fileCollectionEndpoint, null, {
                processData: false,
                headers: {
                    //"accept": "application/json;odata=verbose",
                    //Authorization: "Bearer " + accessToken
                    "X-RequestDigest": $("#__REQUESTDIGEST").val(),
                    "IF-MATCH": "*",
                    "X-HTTP-Method":"DELETE"
                }
            });
    }

    spFormFactory.generateGuid = function () {
        return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
            var r = Math.random()*16|0, v = c == 'x' ? r : (r&0x3|0x8);
            return v.toString(16);
        });
    }

    // Allow child directives each to register a promise for when their post-save actions are completed
    // Track each of their promises and ensure all are resolved before proceeding to the cancel() function
    var postsavepromises = [];
    spFormFactory.registerPostSavePromise = function(p) {
        postsavepromises.push(p);
    }
    /********************************/
    return spFormFactory;

});