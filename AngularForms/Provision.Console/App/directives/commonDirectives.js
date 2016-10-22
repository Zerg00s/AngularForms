(function(){
//The currently executing script file will always be the last
//one in the scripts array, so you can easily find its path:
var scripts = document.getElementsByTagName("script")
var currentScriptPath = scripts[scripts.length-1].src;

app.directive('radioField', function() {
    return {
        templateUrl: currentScriptPath.replace('commonDirectives.js', 'radioField.html'),
        scope: {      
            disable: '=ngDisabled',
            field: '=',
            require: '=ngRequired'
        }
    }
});


app.directive('choiceField', function() {
    return {
        templateUrl: currentScriptPath.replace('commonDirectives.js', 'choiceField.html'),
        scope: {      
            disable: '=ngDisabled',
            field: '=',
            require: '=ngRequired'
        }
    }
});

app.directive('datetimePicker', function() {
    return {
        templateUrl: currentScriptPath.replace('commonDirectives.js', 'datetimePicker.html'),
        scope: {         //  scope initializer....        
            format:  '=', //= means bind it to the outside controller...
            model:   '=ngModel',
            disable: '=ngDisabled',
            require: '=ngRequired'
           // ngDisabled: "=ngDisabled"
        },
        controller: function($scope){
            $scope.buttonClick = function(){
                 $scope.isOpen = true;
            }
        }
    }
});

// it should be able to disable ALL inputs within a certain parent! 
// this way I won't need to retype ng-disable at every control

//TODO: It does not work with the wrapped directives!
app.directive('allInputsDisabled', function () {
    var allInputsDisabled = {};
    allInputsDisabled.link = function (scope, element, attrs) {

        scope.$watch(attrs.allInputsDisabled, function (newValue) {
                $(element).find('input, button, select, option, textarea').prop('disabled', !!newValue);
                //how do I apply attribute changes to the wrapped directive?
            });
   };

   return allInputsDisabled;

});


/* FILE UPLOADED DIRECTIVE */
app.directive('fileUploader', function($parse, $timeout, $q, spFormFactory) {
    return {
        templateUrl: currentScriptPath.replace('commonDirectives.js', 'fileUploader.html'),
        scope: {      
            disable: '=ngDisabled',
            require: '=ngRequired',
            //parentItemId: '=',
            controlId: '=?',
            attachmentDocLibName: '=?',
            attachmentDocFolderName: '=?',
            attachmentFilterFieldName: '=?',
            showDisplayNameField: '='
        },

        link: function(scope, iElem, iAttrs) {
            var model = scope.model = {
                PENDING_CHANGE_TYPE_KEY: "PendingChangeType",
                PendingChangeTypeEnum: {
                    ADD: 1,
                    DELETE: 2
                },

                fileReaderUnsupported: false,
                showDisplayNameField: (typeof(scope.showDisplayNameField) === "boolean" ? scope.showDisplayNameField : true),
                attachmentList: [],
                attachmentDisplayName: "",
                attachmentUniqueName: "",

                getParentItemId: function() {
                    return (spFormFactory ? spFormFactory.itemId : undefined);
                },

                getFileInputControl: function () {
                    var fileInput = $('#' + scope.controlId + " .fileUploadControl");
                    return (fileInput.length <= 0 ? null : fileInput[0]);
                },

                getAttachmentFromList: function(fileuniquename) {
                    if (!model.attachmentList || model.attachmentList.length <= 0) return null;
                    var existingfiles = model.attachmentList.filter(function (f) {
                        return f.File.Name === fileuniquename;
                    });
                    return (existingfiles.length > 0 ? existingfiles[0].File : null);
                },


                // Upload the file.
                // You can upload files up to 2 GB with the REST API.
                uploadFile: function (attachment) {
                    // Define custom metadata properties for the file before saving
                    var customFileMetadata = {};
                    var parentItemId = model.getParentItemId();
                    if (parentItemId && parentItemId != '') customFileMetadata[scope.attachmentFilterFieldName + "Id"] = String(parentItemId);

                    // Upload the file using the SharePoint item service
                    return spFormFactory.uploadFileToDocLib(attachment.File.ArrayBuffer, scope.attachmentDocLibName, scope.attachmentDocFolderName, attachment.File.Name, attachment.File.Title, customFileMetadata);
                },

                addAttachmentPending: function() {
                    var fileInput = model.getFileInputControl();
                    if (!fileInput || fileInput.files.length <= 0) {
                        console.error("No attachment to add or error locating file input control");
                        return;
                    }

                    // Assign a unique name to this file
                    // Eventually, when adding directly to the parent list item, this can just be the file's display name
                    model.attachmentUniqueName = spFormFactory.generateGuid() + fileInput.files[0].name.substr(fileInput.files[0].name.lastIndexOf("."));
                    // model.attachmentUniqueName = model.attachmentDisplayName;

                    // Pre-load the file's contents into memory
                    spFormFactory.getFileBuffer(fileInput)
                    .then(function onSuccess(fileArrayBuffer) {
                        // Prepare the new file object
                        var newfile = model.getAttachmentFromList(model.attachmentUniqueName);
                        if (!newfile) {
                            newfile = { File: { 
                                Name: model.attachmentUniqueName,
                                Title: model.attachmentDisplayName 
                            } };
                        }

                        newfile.File['ArrayBuffer'] = fileArrayBuffer;
                        newfile.File[model.PENDING_CHANGE_TYPE_KEY] = model.PendingChangeTypeEnum.ADD;

                        // Add to the attachment list
                        model.attachmentList.push(newfile);

                        // Reset the file input controls
                        fileInput.value = '';
                        fileInput.type = '';
                        fileInput.type = 'file';
                        
                        model.attachmentDisplayName = '';
                    });
                },

                deleteAttachment: function(attachment) {
                    if (typeof(attachment) !== "object" || attachment === null || typeof(attachment.File) !== "object") {
                        console.error("Unable to locate attachment for deletion");
                        return;
                    }

                    // Mark file for deletion
                    if (model.isServerAttachment(attachment)) attachment.File[model.PENDING_CHANGE_TYPE_KEY] = model.PendingChangeTypeEnum.DELETE;
                },

                cancelPendingAttachmentChange: function(attachment) {
                    if (typeof(attachment) !== "object" || attachment === null || typeof(attachment.File) !== "object") {
                        console.error("Unable to locate attachment for cancellation");
                        return;
                    }

                    if (model.isServerAttachment(attachment)) {
                        // Mark file for deletion
                        attachment.File[model.PENDING_CHANGE_TYPE_KEY] = undefined;

                    } else {
                        // Delete from pending queue immediately
                        var indexes = $.map(model.attachmentList, function(aFile, index) {
                            if (aFile.File.Name === attachment.File.Name) 
                                return index;
                        });
                        if (indexes.length > 0) model.attachmentList.splice(indexes[0], 1);
                    }
                },


                isServerAttachment: function(attachment) {
                    return (typeof(attachment) === "object" && attachment !== null && attachment.File['ServerRelativeUrl'] !== undefined);
                },

                isAttachmentPendingChange: function(attachment) {
                    return (typeof(attachment) === "object" && attachment !== null && attachment.File[model.PENDING_CHANGE_TYPE_KEY] !== undefined);
                },

                isFileSelected: function() {
                    var fileInput = model.getFileInputControl();
                    var fileSelected = (fileInput && fileInput.files && fileInput.files.length > 0);
                    

                    return fileSelected;
                }
            };

            // Ensure file reader API is supported
            if (!window.FileReader) model.fileReaderUnsupported = true;

            scope.controlId = scope.controlId || 'fileuploader';
            scope.attachmentDocLibName = scope.attachmentDocLibName || 'Attachments';
            scope.attachmentFilterFieldName = scope.attachmentFilterFieldName || 'ParentItemID';
            scope.attachmentDocFolderName = scope.attachmentDocFolderName || '';

            // Pre-populate with initial attachment list
            var promise = spFormFactory.loadAttachments(scope.attachmentDocLibName, scope.attachmentDocFolderName, scope.attachmentFilterFieldName, model.getParentItemId());
            if (typeof(promise) === "object" && promise !== null) {
                promise.then(function (requestResponse) {
                    model.attachmentList = requestResponse;
                });
            }

            // Post Save Action
            scope.$on('spFormFactory:itemSaved', function (event, promise) {
                model.attachmentList.forEach(function(attachment) {                          
                    // If no change pending for this attachment, skip it
                    if (!model.isAttachmentPendingChange(attachment)) return;
                
                    // If attachment is pending an addition or deletion, call the SP service to upload it and register its promise with the service
                    if (attachment.File[model.PENDING_CHANGE_TYPE_KEY] === model.PendingChangeTypeEnum.ADD) {
                        spFormFactory.registerPostSavePromise(promise.then(function() { return model.uploadFile(attachment); }));

                    } else if (attachment.File[model.PENDING_CHANGE_TYPE_KEY] === model.PendingChangeTypeEnum.DELETE) {
                        spFormFactory.registerPostSavePromise(promise.then(function() { return spFormFactory.deleteFileFromDocLib(scope.attachmentDocLibName, scope.attachmentDocFolderName, attachment.File.Name); }));
                    }
                });
            });
        },

        controller: ['$scope', function fileUploaderController($scope) {
            this.onFileSelected = function () {
                // Grab the file name from the uploader control and populate into the display name field
                var fileInput = $scope.model.getFileInputControl();
                if (!fileInput || !fileInput.files || fileInput.files.length <= 0) return;
                if (typeof($scope.model.attachmentDisplayName) !== "string" || $scope.model.attachmentDisplayName.length <= 0 || !$scope.model.showDisplayNameField) {
                    $scope.model.attachmentDisplayName = fileInput.files[0].name;
                }
                
                if ($scope.model.showDisplayNameField == false) {
                    $scope.model.addAttachmentPending();
                } 
            }
        }]
    }
});

// Handles pre-population of the file display name after one is selected in the file input control
app.directive('onFileChanged', function ($parse) {
    return {
        // This syntax allows calls up to the parent fileUploader controller
        require: '^^fileUploader',

        link: function (scope, iElem, iAttrs, fileUploaderCtrl) {
            iElem.on('change', function () {
                var getter = $parse(iAttrs.onFileChanged);
                var returnValue = getter(scope, {});

                

            });

            scope.updateDisplayName = function() {
                // Call the parent fileUploader controller's onFileSelected() function to change the file name since the display name field is not visible in this directive
                fileUploaderCtrl.onFileSelected();

                scope.$apply();
            }
        }
    };
});


})()
