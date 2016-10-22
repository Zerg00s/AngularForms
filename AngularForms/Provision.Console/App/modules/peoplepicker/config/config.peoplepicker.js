(function () {
    angular.module('ui.People', [])
    .value('uiPeopleConfig', {})
    .directive('uiPeople', ['uiPeopleConfig', '$q', '$injector', '$resource', '$timeout', function (uiPeopleConfig, $q, $injector, $resource, $timeout) {
        uiPeopleConfig = uiPeopleConfig || {};
        uiPeopleConfig.validatingUsers = 'uiPeopleConfig.validatingUsers';
        uiPeopleConfig.validatedUsers = 'uiPeopleConfig.validatedUsers';
        var generatedIds = 0;
        return {
            require: 'ngModel',
            priority: 10,
            template: '<div id="{{peoplePickerParent}}"><div id="{{peoplePickerId}}"></div></div><div class="ui-people-readonly" style="display:none" id="{{peoplePickerTextId}}" ><span data-ng-bind-html="users"></span></div>',
            scope: true,
            link: function (scope, elm, attrs, ngModel) {
                var rootScope = null;
                if (!rootScope) {
                    rootScope = $injector.get("$rootScope");
                }
                if (!attrs.id) {
                    attrs.$set('id', 'uiPeople' + generatedIds++);
                }
                scope.users = "";
                var securityValidation = '';
                var loading = true;                
                var scriptsLoaded = false;
                var clientCtx = null;
                var isMultiValued = attrs.ppIsMultiuser ? scope.$eval(attrs.ppIsMultiuser) : false;
                var principalAccountType = (attrs.ppAccountType && attrs.ppAccountType.match(/^(?:user)|(?:dl)|(?:secgroup)|(?:spgroup)$/i)) ? attrs.ppAccountType : 'User,DL,SecGroup,SPGroup';
                var pickerWidth = (attrs.ppWidth && attrs.ppWidth.match(/^[0-9][0-9]*px$/i)) ? attrs.ppWidth : '220px';

                scope.isDisabled = attrs.ngDisabled ? scope.$eval(attrs.ngDisabled) : false;
                scope.currentWebUrl = attrs.ppWebUrl ? scope.$eval(attrs.ppWebUrl) : _spPageContextInfo.webAbsoluteUrl;
                scope.text = '';
                scope.peoplePickerParent = ('peoplePickerParent_' + attrs.id);
                scope.peoplePickerId = ('peoplePicker_' + attrs.id);
                scope.peoplePickerTextId = ('text_' + attrs.id);
				
				if(attrs.hasOwnProperty('ppWebUrl')){					
					attrs.$observe('ppWebUrl', function(value) {						
						scope.currentWebUrl = value ? value : _spPageContextInfo.webAbsoluteUrl
					})
				}
				
                var models = {};
                models.pickerSchema = function (accountType, multipleValues, width) {
                    this.pickerSchema = {};
                    this.PrincipalAccountType = accountType ? accountType : 'User,DL,SecGroup,SPGroup';
                    this.SearchPrincipalSource = 15;
                    this.ResolvePrincipalSource = 15;
                    this.AllowMultipleValues = multipleValues == true ? true : false;
                    this.MaximumEntitySuggestions = 50;
                    this.Width = width ? width : '220px';
                    this.OnvalidatedUsersClientScript = null;
                };
                models.userSchema = function (userName, displayName, userId, isResolved, entityType) {
                    this.AutoFillDisplayText = displayName ? displayName : null;
                    this.AutoFillKey = userName ? userName : null;
                    this.AutoFillSubDisplayText = "";
                    this.DisplayText = displayName ? displayName : null;
                    this.EntityType = entityType ? entityType : "User";
                    this.IsResolved = isResolved === true ? true : false;
                    this.Key = userName ? userName : null;;
                    this.ProviderDisplayName = "Tenant";
                    this.ProviderName = "Tenant";
                    this.Resolved = true;
                    this.userId = userId ? userId : null;
                };
                models.ensureUser = function (userName) {
                    this.logonName = userName ? userName : null;
                };
                models.userCollection = function (multiUserPicker, webUrl) {
                    this._pendingUsers = [];
                    this._currentUsers = [];
                    this._webUrl = webUrl ? webUrl : _spPageContextInfo.webAbsoluteUrl;
                    this._multi = multiUserPicker === true ? true : false;
                };
                models.userCollection.prototype.updateFromViewValue = function (value, scope) {
                    this.deleteCurrentUsers();
                    var pickerId = scope.peoplePickerId + '_TopSpan';
                    if (SPClientPeoplePicker.SPClientPeoplePickerDict.hasOwnProperty(pickerId) && !scope.isDisabled) {
                        var currentUsers = SPClientPeoplePicker.SPClientPeoplePickerDict[pickerId].DeleteProcessedUser();
                    }
                    if (value && value.results instanceof Array) {
                        for (var i = 0; i < value.results.length; i++) {
                            var cu = value.results[i];
                            if (cu.Name && cu.Title) {
                                var user = new models.userSchema(cu.Name, cu.Title, cu.Id, true);
                                this._currentUsers.push(user);
                                if (SPClientPeoplePicker.SPClientPeoplePickerDict.hasOwnProperty(pickerId) && !scope.isDisabled) {
                                    SPClientPeoplePicker.SPClientPeoplePickerDict[pickerId].AddResolvedUserToLocalCache(user);
                                    SPClientPeoplePicker.SPClientPeoplePickerDict[pickerId].AddProcessedUser(user);
                                }
                            }
                        }
                        if (SPClientPeoplePicker.SPClientPeoplePickerDict.hasOwnProperty(pickerId)) {
                            //SPClientPeoplePicker.SPClientPeoplePickerDict[pickerId].ResolveAllUsers(false);	
							//This causes setUserIdFromPickerChoice to be called, which goes through validating users, and then updates the model
							//Need to find an improved way of handling this.
                        }
                    }
                    else if (value) {
                        if (value.Name && value.Title) {
                            var user = new models.userSchema(value.Name, value.Title, value.Id, true);
                            this._currentUsers.push(user);
                            if (SPClientPeoplePicker.SPClientPeoplePickerDict.hasOwnProperty(pickerId) && !scope.isDisabled) {
                                SPClientPeoplePicker.SPClientPeoplePickerDict[pickerId].AddResolvedUserToLocalCache(user);
                                SPClientPeoplePicker.SPClientPeoplePickerDict[pickerId].AddProcessedUser(user);
                                //SPClientPeoplePicker.SPClientPeoplePickerDict[pickerId].ResolveAllUsers();	
								//This causes setUserIdFromPickerChoice to be called, which goes through validating users, and then updates the model
								//Need to find an improved way of handling this.								
                            }
                        }
                    }

                };
                models.userCollection.prototype.updateFromPickerUserKeys = function (value) {
                    this.deleteCurrentUsers();
                    if (value && value instanceof Array) {
                        for (var i = 0; i < value.length; i++) {
                            var cu = value[i];
                            if (cu.Key && (cu.AutoFillDisplayText || cu.DisplayText)) {
                                var user = null;
                                if (cu.EntityData && cu.EntityData.SPGroupID) {
                                    user = new models.userSchema(cu.Key, (cu.AutoFillDisplayText || cu.DisplayText), cu.EntityData.SPGroupID, cu.Resolved, 'SPGroup');
                                } else {
                                    user = new models.userSchema(cu.Key, (cu.AutoFillDisplayText || cu.DisplayText), null, cu.Resolved);
                                }
                                this._currentUsers.push(user);
                            }
                        }
                    }
                };
                models.userCollection.prototype.getViewValue = function () {
                    if (this._multi === true) {
                        var uc = {};
                        uc.results = [];
                        for (var i = 0; i < this._currentUsers.length; i++) {
                            var u = this._currentUsers[i];
                            uc.results.push({ Name: u.Key, Title: (u.AutoFillDisplayText || u.DisplayText), Id: u.userId })
                        }
                        return uc;
                    } else {
                        var u = this._currentUsers.length > 0 ? this._currentUsers[0] : null;
                        return u === null ? null : { Name: u.Key, Title: (u.AutoFillDisplayText || u.DisplayText), Id: u.userId };
                    }
                };
                models.userCollection.prototype.getUsersAsHashTable = function () {
                    var h = {};
                    for (var i = 0; i < this._currentUsers.length; i++) {
                        var u = this._currentUsers[i];
                        h[(u.Key)] = u.Key;
                    }
                    return h;
                };
                models.userCollection.prototype.getPendingUsers = function () {
                    return this._pendingUsers;
                };
                models.userCollection.prototype.removePendingUser = function (index) {
                    this._pendingUsers.splice(index, 1);
                };
                models.userCollection.prototype.removeCurrentUser = function (index) {
                    this._currentUsers.splice(index, 1);
                };
                models.userCollection.prototype.deleteCurrentUsers = function () {
                    this._currentUsers.length = 0;
                };
                models.userCollection.prototype.getUserIdResource = function () {
                    return $resource(this._webUrl + '/_api/web/ensureuser',
                       {}, {
                           post: {
                               method: 'POST',
                               params: {
                               },
                               headers: {
                                   'Accept': 'application/json;odata=verbose',
                                   'Content-Type': 'application/json;odata=verbose;',
                                   'X-RequestDigest': securityValidation
                               }
                           }
                       });
                };
                models.userCollection.prototype.resolveUsers = function () {
                    var deferred = $q.defer();
                    if (this._pendingUsers.length > 0) {
                        for (var i = (this._pendingUsers.length - 1) ; i >= 0; i--) {
                            var pu = this._pendingUsers[i];
                            if (pu.EntityType === 'SPGroup') {
                                $timeout(function () {
                                    userModel._pendingUsers.pop();
                                    pu.Resolved = true;
                                    userModel._currentUsers.push(pu);
                                    if (userModel._pendingUsers.length === 0) {
                                        deferred.resolve();
                                    }
                                }, 3500);
                            } else {
                                var spUserModel = new models.ensureUser(pu.Key);
                                this.getUserIdResource().post(spUserModel, function (data) {
                                    userModel._pendingUsers.pop();                                    
                                    if (data && data.d && data.d.Id && data.d.LoginName) {
                                        var user = new models.userSchema(data.d.LoginName, data.d.Title, data.d.Id, true);
                                        userModel._currentUsers.push(user);
                                    }
                                    if (userModel._pendingUsers.length === 0) {
                                        deferred.resolve();
                                    }
                                }, function (error) {
                                    var message = 'Failed to ensure user. Exception: ' + error.statusText;
                                    deferred.reject(message);
                                });
                            }
                        }
                    } else {
                        $timeout(function () {
                            deferred.resolve();
                        }, 100);
                    }
                    return deferred.promise;
                };
                models.userCollection.prototype.renderUserWithPresence = function (userId, userTitle) {
                    if (userId && userTitle) {
                        return '<div class="ui-people-userlink"><span class="ms-noWrap"><span class="ms-spimn-presenceLink"><span class="ms-spimn-presenceWrapper ms-imnImg ms-spimn-imgSize-10x10"><img class="ms-spimn-img ms-spimn-presence-disconnected-10x10x32" src="' + this._webUrl + '/_layouts/15/images/spimn.png?rev=23"  alt="" /></span></span><span class="ms-noWrap ms-imnSpan"><span class="ms-spimn-presenceLink"><img class="ms-hide" src="' + this._webUrl + '/_layouts/15/images/blank.gif?rev=23"  alt="" /></span><a class="ms-subtleLink" onclick="GoToLinkOrDialogNewWindow(this);return false;" href="' + this._webUrl + '/_layouts/15/userdisp.aspx?ID=' + userId + '">' + userTitle + '</a></span></span></div>';
                    }
                    return '<div class="ui-people-userlink"><span class="ms-noWrap">' + userTitle + '</span></div>';
                }
                models.userCollection.prototype.renderUsersAsReadOnly = function (model) {
                    var usersHtml = "";
                    for (var i = 0; i < this._currentUsers.length; i++) {
                        var u = this._currentUsers[i];
                        usersHtml = usersHtml + this.renderUserWithPresence(u.userId, u.DisplayText);
                    }
                    return usersHtml;
                }

                var updateView = function () {
                    ngModel.$setViewValue(userModel.getViewValue());
                    if (!scope.$root.$$phase) {
                        scope.$apply();
                    }
                };
                var getViewValue = function () {
                    return ngModel.$modelValue;
                }
                ngModel.$render = Function.createDelegate(scope, function () {
                    scope.isDisabled = attrs.ngDisabled ? scope.$eval(attrs.ngDisabled) : false;
                    userModel.updateFromViewValue(ngModel.$modelValue, scope);
                    if (scope.isDisabled) {
                        scope.users = userModel.renderUsersAsReadOnly();
                    }
                    toggleReadOrEditField(scope);
                    return;
                });

                var userModel = new models.userCollection(isMultiValued);

                function init(scope) {
                    ensureScriptsLoaded(scope).then(function (data) {
                        refreshSecurityValidation();
                        scriptsLoaded = true;
                        initializePeoplePicker(scope);
                    })["catch"](function (error) {
                        //log error
                    });
                };

                function initializePeoplePicker(scope) {
                    var schema = new models.pickerSchema(principalAccountType, isMultiValued, pickerWidth);
                    schema.OnUserResolvedClientScript = setUserIdFromPickerChoice;
                    SPClientPeoplePicker_InitStandaloneControlWrapper(scope.peoplePickerId, userModel._currentUsers, schema);
                    //If you don't need to support IE8, then you can add the users while initialising the picker control, like this:
                    //SPClientPeoplePicker_InitStandaloneControlWrapper(peoplePickerElementId, users, schema);
                    //However, if you're using IE8, this doesn't work - I think it's to do with the Array prototype extension forEach
                    //Here's the workaround - loop through the users and add them manually.
                    //SPClientPeoplePicker_InitStandaloneControlWrapper(peoplePickerInstance.id, null, schema);
                    //var peoplePicker = SPClientPeoplePicker.SPClientPeoplePickerDict[(scope.peoplePickerId + "_TopSpan")];
                    //for (var i = 0; i < users.length; i++) {
                    //	var u = users[i];
                    //	peoplePicker.AddProcessedUser(u);
                    //}	
                    loading = false;
                    if (scope.isDisabled) {
                        scope.users = userModel.renderUsersAsReadOnly();
                    }
                    toggleReadOrEditField(scope);
                }

                function setUserIdFromPickerChoice(elementId, userKeys) {
                    //If the directive is loading users during initialisation, we don't need to call Ensure user or update the model (the users are coming from the model during init).  
                    if (loading) { return };
                    rootScope.$broadcast(uiPeopleConfig.validatingUsers, { message: 'Validating users', value: userKeys, show: true });
                    var newUserCollection = new models.userCollection(isMultiValued);
                    newUserCollection.updateFromPickerUserKeys(userKeys);
                    var newUserHashTable = newUserCollection.getUsersAsHashTable();
                    var indexLength = userModel._currentUsers.length;
                    //Remove users from the currentUsers array that aren't present in the userKeys
                    for (var i = (indexLength - 1) ; i >= 0; i--) {
                        var u = userModel._currentUsers[i];
                        if (!(newUserHashTable[(u.Key)])) {
                            userModel.removeCurrentUser(i);
                        }
                    }

                    //Get all the new users in the userKeys array
                    var currentUserHashTable = userModel.getUsersAsHashTable();
                    indexLength = newUserCollection._currentUsers.length;
                    for (var i = (indexLength - 1) ; i >= 0; i--) {
                        var u = newUserCollection._currentUsers[i];
                        if (!(currentUserHashTable[(u.Key)])) {
                            //User not found in the existing currentUsers collection. Add them to the pending users collection.                            
                            userModel._pendingUsers.push(u);
                        }
                    }

                    var defer = $q.defer();
                    userModel.resolveUsers().then(function (value) {
                        updateView();
                        rootScope.$broadcast(uiPeopleConfig.validatedUsers, { message: 'Validated users', value: userKeys, show: false });
                        return defer.resolve(value);
                    })["catch"](function (error) {
                        rootScope.$broadcast(uiPeopleConfig.validatedUsers, { message: 'Failed to validated users', value: userKeys, show: false });
                        defer.reject(error);
                    });
                    return defer.promise;
                }

                function refreshSecurityValidation() {
                    var siteCtxInfoResource = $resource(scope.currentWebUrl + '/_api/contextinfo', {}, {
                        post: {
                            method: 'POST',
                            headers: {
                                'Accept': 'application/json;odata=verbose',
                                'Content-Type': 'application/json;odata=verbose'
                            }
                        }
                    });

                    siteCtxInfoResource.post({},
                       function (data) {
                           //callback success. Get digest timeout and value, and store it in the service
                           var siteCtxInfo = data.d.GetContextWebInformation;
                           var validationRefreshTimeout = siteCtxInfo.FormDigestTimeoutSeconds - 10;
                           securityValidation = siteCtxInfo.FormDigestValue;
                           //repeat the validation refresh in timeout
                           $timeout(function () {
                               refreshSecurityValidation();
                           }, validationRefreshTimeout * 1000);
                       },
                       function (error) {
                       });
                }

                function ensureScriptsLoaded(scope) {
                    var deferred = $q.defer();
                    if (SP.ClientContext === undefined) {
                        SP.SOD.executeFunc('sp.js', 'SP.ClientContext', function () {
                            clientCtx = SP.ClientContext.get_current();
                            if (!scope.currentWebUrl) {
                                scope.currentWebUrl = clientCtx.get_url();
                            }
                            deferred.resolve(true);
                        });
                    } else {
                        $timeout(function () {
                            clientCtx = SP.ClientContext.get_current();
                            if (!scope.currentWebUrl) {
                                scope.currentWebUrl = clientCtx.get_url();
                            }
                            deferred.resolve(true);
                        }, 250);
                    }
                    return deferred.promise;
                }

                function toggleReadOrEditField(scope) {
                    var p = document.getElementById(scope.peoplePickerParent);
                    var e = document.getElementById(scope.peoplePickerTextId);
                    if (p && e) {
                        if (scope.isDisabled) {                            			
                            e.style.display = 'block';
                            p.style.display = 'none';
                        } else {
                            e.innerHTML = '';
                            e.style.display = 'none';
                            p.style.display = 'block';
                        }
                    }
                }

                init(scope);
            }
        };
    }]);
})();
