"use strict";
//ROOM RESERVATIONS FORM: 
app.controller('MainCtrl', function ($scope, $http, $log, $location, spFormFactory) {    
    $scope.spFormFactory = spFormFactory;

    spFormFactory.initialize($scope, 'Senate Room Reservation').then(init);
    $scope.calendarFormat = 'dd-MMMM-yyyy';
    $scope.userId = _spPageContextInfo.userId;
    $scope.moment = window.moment;

    if (window.location.href.indexOf('NewForm.aspx') != -1){
        $scope.CurrentUserPartOfWorkflow = true;
    }

    $scope.saveWithButtonDisabled = function(){
        $scope.hideSaveButton = 'true';
        $scope.save();
    }

    function init() {
        
        //Business logic goes here:
        if($scope.formMode == 'New') {      
            $scope.CurrentUserPartOfWorkflow = true;
            //Select current user as a Requestor by default
            var getUserRestURL = _spPageContextInfo.webServerRelativeUrl + "/_api/Web/GetUserById(" + _spPageContextInfo.userId + ")";
            $http.get(getUserRestURL).then(function(requestResponse) {
                $scope.f.Title.Value = requestResponse.data.d.Title;
                $scope.f.ContactPerson.Value = requestResponse.data.d.Title;
            });
        }
        else{

            if ($.inArray($scope.userId, [$scope.f.Author.Value, $scope.f.ContactPerson.Value, $scope.f.OptionalApproverRoom.Value ]) > -1 ){
                $scope.CurrentUserPartOfWorkflow = true;
            }
            else{
                $scope.CurrentUserPartOfWorkflow = false;
            }
        }

        $scope.$watch('[f.StartTime.Value, f.EndTime.Value]',function(){
            $scope.f.StartTimeString.Value =  moment($scope.f.StartTime.Value).format("hh:mm:ss a");
            $scope.f.EndTimeString.Value = moment($scope.f.EndTime.Value).format("hh:mm:ss a");
        
         // Validating that End time should not be earlier than the Start time

            if($scope.f.EndTime.Value < $scope.f.StartTime.Value ){
                $scope.mainForm.StartTime.$setValidity("not valid", false);
                $scope.mainForm.EndTime.$setValidity("not valid", false);
            }
            else
            {
                $scope.mainForm.StartTime.$setValidity("not valid", true);
                $scope.mainForm.EndTime.$setValidity("not valid", true);
            }
        });
        
        populateCurrentUserAsAcontact();
        setDefaultValues();
        determineCurrentStage();   
        getuserMembership()  
        .then(parseUserMembership)
        .then(getUserRoles)
        .then(determineIfCurrentuserCanEditAnything);
    }

    function populateCurrentUserAsAcontact(){
        if($scope.formMode == 'New'){
            //Select current user as a Requestor by default
            var getUserRestURL = _spPageContextInfo.webServerRelativeUrl + "/_api/Web/GetUserById(" + _spPageContextInfo.userId + ")";
            $http.get(getUserRestURL).then(function(requestResponse) {
                $scope.f.ContactPerson.user = {
                    Name: requestResponse.data.d.LoginName,
                    Id: requestResponse.data.d.Id, 
                    Title:requestResponse.data.d.Title
                }
            });
        }
    }

    function setDefaultValues(){
        if($scope.formMode == 'New'){
             $scope.f.StartTime.Value = moment().second(0).milliseconds(0).toDate();
             $scope.f.EndTime.Value = moment().second(0).milliseconds(0).toDate();
             $scope.f.DateRequested.Value = moment().toDate();
             $scope.f.DateOfUse.Value = moment().toDate();
             $scope.f.NumberOfAttendees.Value = 1;
        }
        else{
            if($scope.f.AcFrom.Value == null || $scope.f.AcFrom.Value.toString().indexOf('nvalid') != -1 ){
                $scope.f.AcFrom.Value = '';
            }
            if($scope.f.AcTo.Value == null || $scope.f.AcTo.Value.toString().indexOf('nvalid') != -1){
                $scope.f.AcTo.Value = '';
            }
        }
    }

    function getuserMembership() {
        var restQueryUrl = _spPageContextInfo.webServerRelativeUrl +
            "/_api/web/currentUser?$select=Groups/Title&$expand=Groups";
        return $http.get(restQueryUrl);
    };

    function determineIfCurrentuserCanEditAnything() {
        if($scope.isOptionalApprover && $scope.OptionalStage){
            $scope.OptionalApproverCanEditNow = true;
            $scope.CurrentUserPartOfWorkflow = true;
        }
        else if($scope.isRoomApprover && $scope.RoomApproverStage){
            $scope.RoomApproverCanEditNow = true;
            $scope.CurrentUserPartOfWorkflow = true;
        }
        else {
            if($scope.formMode != 'New'){
                $scope.saveButtonDisabled = true;
            }
        }
    }

    function determineCurrentStage(){
        if($scope.f.RoomApprovalStatus.Value == null){
            return;
        }
        if ($scope.f.RoomApprovalStatus.Value == 'Waiting for Optional Approver'){
                $scope.OptionalStage = true;
        }
        else if($scope.f.RoomApprovalStatus.Value.toString().indexOf('Waiting') != -1 && $scope.f.RoomApprovalStatus.Value.toString().indexOf('Optional') == -1){
             $scope.RoomApproverStage = true;
        }
        else if($scope.f.RoomApprovalStatus.Value.toString().indexOf('Approved') != -1 ){
            $scope.ClosedStage = true;  
        }
        else if($scope.f.RoomApprovalStatus.Value.toString().indexOf('Declined') != -1){
            $scope.ClosedStage = true;
        }
    }

    function parseUserMembership(promise) {
        $scope.Groups = [];

        if($scope.formMode == 'New'){
            return promise;
        }
        
        for (var k = 0; k < promise.data.d.Groups.results.length; k++) {
            var groupName = promise.data.d.Groups.results[k].Title;
            $scope.Groups.push(groupName);
            var room = $scope.f.UseOfRoomOrChamber.Value.trim();
            if (groupName == 'WAM' && room == "Room 211"){
                 $scope.isWAM = true;
                 $scope.isRoomApprover  = true;
            }            
            else if (groupName == 'Senate President' && room == "Chamber"){
                $scope.isPresident = true;
                $scope.isRoomApprover  = true;
            }
            else if (groupName == 'Majority Caucus Leader' && room == "Majority Caucus Room"){
                $scope.isMajorityLeader = true;  
                $scope.isRoomApprover  = true;              
            }
            else if (groupName == 'Minority Caucus Leader' && room == "Minority Caucus Room"){
                $scope.isMinorityLeader = true;   
                $scope.isRoomApprover  = true;             
            }
            else if (groupName == 'SGT-at-Arms'){
                 $scope.isSGT = true;        
                 $scope.isRoomApprover  = true;        
            }
        }
        return promise;
    }//parseUserMembership end

    function getUserRoles(promise){
        if ($scope.f.OptionalApproverRoom.Value == $scope.userId){
            $scope.isOptionalApprover = true;
        }
        return promise;
    }

});