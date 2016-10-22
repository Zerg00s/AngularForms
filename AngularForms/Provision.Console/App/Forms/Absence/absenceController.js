"use strict";
app.controller('MainCtrl', function ($scope, $http, $log, $location, $interpolate, $compile, spFormFactory) {
    $scope.spFormFactory = spFormFactory;
    spFormFactory.initialize($scope, 'Leave of Absence').then(init);
    $scope.calendarFormat = 'dd-MMMM-yyyy';
    $scope.userId = _spPageContextInfo.userId;
    $scope.moment = window.moment;

    if (window.location.href.indexOf('NewForm.aspx') != -1){
        $scope.CurrentUserPartOfWorkflow = true;
    }

    function init() {
        //Business logic goes here:      
        $scope.$watch('f.Requestor.user',updateTitle);  
        $scope.$watch('[f.NumberOfDays.Value, f.NumberOfDaysFraction.Value]', updateNumberOfHours);
        $scope.$watch('f.WithPay.Value',function(){
            if($scope.f.WithPay.Value == 'No'){
                $scope.f.TypeOfLeave.Value = null;
            }
        });

        populateCurrentUserAsArequestor();         
        setDefaultValues();
        determineCurrentStage();
        
        getuserMembership() 
        .then(parseUserMembership)
        .then(getUserRoles)
        .then(determineIfCurrentuserCanEditAnything);
    }

    spFormFactory.onPostSaveAction = function(promise) { 
        return promise;        
    }

    function updateNumberOfHours(){
        $scope.f.NumberOfHours.Value = parseFloat($scope.f.NumberOfDays.Value) + parseFloat($scope.f.NumberOfDaysFraction.Value);
    }

    function updateTitle(){ 
        if($scope.f.Requestor.user){
            $scope.f.Title.Value = $scope.f.Requestor.user.Title; 
        }
    }

    function setDefaultValues(){
        if($scope.formMode == 'New'){
            $scope.f.NumberOfDays.Value = 0;
            $scope.f.NumberOfDaysFraction.Value = '0';
            $scope.f.WithPay.Value = 'Yes';
            $scope.f.WFStatus.Value = 'Waiting for Manager Approver';            
        }
        else{
            if ($.inArray($scope.userId, [$scope.f.Author.Value, $scope.f.Manager.Value, $scope.f.OptionalApproverUser.Value]) > -1 ){
                $scope.CurrentUserPartOfWorkflow = true;
            }
            else{
                $scope.CurrentUserPartOfWorkflow = false;
            }
        }
    }

    function populateCurrentUserAsArequestor(){
        if($scope.formMode == 'New'){
            //Select current user as a Requestor by default
            var getUserRestURL = _spPageContextInfo.webServerRelativeUrl + "/_api/Web/GetUserById(" + _spPageContextInfo.userId + ")";
            $http.get(getUserRestURL).then(function(requestResponse) {
                $scope.f.Requestor.user = {
                    Name: requestResponse.data.d.LoginName,
                    Id: requestResponse.data.d.Id, 
                    Title:requestResponse.data.d.Title
                }
            });
        }
    }

    function determineCurrentStage(){
        if ($scope.formMode == 'New'){
            return ;
        }
         if ($scope.f.WFStatus.Value == 'Waiting for Manager Approver'){
            $scope.StageNumber = 1;
            $scope.ManagerStage = true;            
        }
        else if($scope.f.WFStatus.Value == 'Waiting for Optional Approver'  ){
                $scope.OptionalStage = true;
                $scope.StageNumber = 2;             
        }
        else if($scope.f.WFStatus.Value == 'Waiting for Accounting Approver'  ){
            $scope.StageNumber = 3;
            $scope.AccountantStage = true;
        }
        else if($scope.f.WFStatus.Value == 'Approved' ){
            $scope.StageNumber = 4;
            $scope.ClosedStage = true;
        }
        else if($scope.f.WFStatus.Value.toString().indexOf('Declined') != -1){
            $scope.StageNumber = 10;
            $scope.ClosedStage = true;
        }

        if ($scope.f.ManagerApproval.Value){
            $scope.managerFieldsFilled = true;
        }
        if ($scope.f.OptionalApproval.Value){
            $scope.optionalFieldsFilled = true;
        }
        if ($scope.f.AccountingApproval.Value){
            $scope.accountingFieldsFilled = true;
        }
    }

    function determineIfCurrentuserCanEditAnything() {
        if($scope.isManager && $scope.ManagerStage && $scope.managerFieldsFilled != true){
            $scope.ManagerCanEditNow = true;
             $scope.CurrentUserPartOfWorkflow = true;
        }
        else if($scope.isOptionalApprover && $scope.OptionalStage  && $scope.optionalFieldsFilled != true){
            $scope.OptionalApproverCanEditNow = true;
             $scope.CurrentUserPartOfWorkflow = true;
        }
        else if($scope.isAccountant && $scope.AccountantStage && $scope.accountingFieldsFilled != true){
            $scope.AccountantCanEditNow = true;
            $scope.CurrentUserPartOfWorkflow = true;
        }
        else {
            if($scope.formMode != 'New'){
                $scope.saveButtonDisabled = true;                
            }
        }
        $scope.formLoaded = true;
    }

    function getUserRoles(promise){
        if ($scope.f.Manager.Value == $scope.userId){
            $scope.isManager = true;
        }
        if ($scope.f.OptionalApproverUser.Value == $scope.userId){
            $scope.isOptionalApprover = true;
        }
        return promise;
    }

    function getuserMembership() {
        var restQueryUrl = _spPageContextInfo.webServerRelativeUrl +
            "/_api/web/currentUser?$select=Groups/Title&$expand=Groups";
        return $http.get(restQueryUrl);
    };

    function parseUserMembership(promise) {
        $scope.Groups = [];
        
        for (var k = 0; k < promise.data.d.Groups.results.length; k++) {
            var groupName = promise.data.d.Groups.results[k].Title;
            $scope.Groups.push(groupName);
            if (groupName == 'Accounting'){
                $scope.isAccountant = true;
                $scope.CurrentUserPartOfWorkflow = true;
            }
        }

        return promise;
    }//parseUserMembership end

});