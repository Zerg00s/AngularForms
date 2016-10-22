"use strict";
app.controller('MainCtrl', function ($scope, $http, $log, $location,spFormFactory) {
    $scope.spFormFactory = spFormFactory;
    $scope.spFormFactory.loading = true;
    spFormFactory.initialize($scope, 'Travel Requests').then(init);
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

    function init(){
       //Add your logic HERE
        $scope.$watch('f.Traveler.user', updateTitle);
        $scope.$watch("[f.TypeTravel.Value, f.NumTrips.Value, f.TypeTravel.Value]", function(){
            if(($scope.f.TypeTravel.Value == 'Home Island' && $scope.f.NumTrips.Value == 'Additional') || $scope.f.TypeTravel.Value == 'Other'){
                $scope.ShowAddionalTravelInfo = true;
            }
            else{
                $scope.ShowAddionalTravelInfo = false;
            }             
        });

        $scope.$watch('f.TypeTravel.Value', function(){
            if ($scope.f.TypeTravel.Value == 'Other'){
                $scope.ShowEstimates = true;
            }
        });

        $scope.$watch('[isTraveler, ClosedStage]',function(){
            if (!($scope.isTraveler) || $scope.ClosedStage){
                $scope.DisableReimbursement = true;
            }
            else{
                $scope.DisableReimbursement = false;
            }
        })

 

        populateCurrentUserAsArequestor(); 
        setDefaultValues();
        determineCurrentStage();
        getuserMembership()  
        .then(parseUserMembership)
        .then(determineIfCurrentuserCanEditAnything);

        $scope.disabled = true;
    }

    $scope.update = function(){
        $scope.disabled = !($scope.disabled); 
        console.log($scope.disabled);
    }

   
    function updateTitle(){ 
        if($scope.f.Traveler.user){
            $scope.f.Title.Value = $scope.f.Traveler.user.Title; 

            if(spFormFactory.userId == $scope.f.Traveler.user.Id){
                $scope.isTraveler = true;
            }
        }
    }

    function determineIfCurrentuserCanEditAnything() {
        if($scope.isPresident && $scope.PresidentStage ){
            $scope.PresidentCanEditNow = true;
        }
        else if($scope.isClerk && $scope.ClerkStage){
            $scope.ClerkCanEditNow = true;
        }
        else if($scope.isAccounting && $scope.AccountingStage){
            $scope.AccountingCanEditNow = true;
        }
        else {
            if($scope.formMode != 'New'){
                $scope.saveButtonDisabled = true;
            }
        }
    }

    function getuserMembership() {
        var restQueryUrl = _spPageContextInfo.webServerRelativeUrl +
            "/_api/web/currentUser?$select=Groups/Title&$expand=Groups";
        return $http.get(restQueryUrl);
    };

    function parseUserMembership(promise) {
    $scope.Groups = [];

    if($scope.formMode == 'New'){
        return promise;
    }
    
    for (var k = 0; k < promise.data.d.Groups.results.length; k++) {
        var groupName = promise.data.d.Groups.results[k].Title;
        $scope.Groups.push(groupName);
        if (groupName == 'Accounting'){
                $scope.isAccounting = true;
        }            
        else if (groupName == 'Senate President'){
            $scope.isPresident = true;
        }
        else if (groupName == 'Travel Request Clerk' ){
            $scope.isClerk = true;  
        }
    }
    return promise;
}//parseUserMembership end
    function determineCurrentStage(){
        if ($scope.f.TravelStatus.Value == null){
            return;
        }
        if ($scope.f.TravelStatus.Value == 'Pending Senate President Approval') {
            $scope.PresidentStage = true;             
        }
        else if ($scope.f.TravelStatus.Value == "Pending Clerk Approval"){
            $scope.ClerkStage = true;
        }
        else if ($scope.f.TravelStatus.Value == "Waiting for Accounting"){
            $scope.AccountingStage = true;
        }
        else if ($scope.f.TravelStatus.Value == "Closed by Accounting"){
            $scope.ClosedStage = true;
            $scope.hideSaveButton = true;
        }
        else if ($scope.f.TravelStatus.Value.indexOf('Disapproved' != -1)){
            $scope.ClosedStage = true;  
            $scope.hideSaveButton = true;      
        }
    }
    function setDefaultValues(){
        if($scope.formMode == 'New'){
            $scope.f.EstAirFare.Value = 0;
            $scope.f.EstHotelLod.Value = 0;
            $scope.f.EstMeals.Value = 0;
            $scope.f.EstRegFee.Value = 0;
            $scope.f.EstGrndTrans.Value = 0;
            $scope.f.EstOtherVal.Value = 0;

            $scope.f.ReimAirFare.Value = 0;
            $scope.f.ReimAirHotelLod.Value = 0;
            $scope.f.ReimAirMeals.Value = 0;
            $scope.f.ReimAirRegFee.Value = 0;
            $scope.f.ReimAirGrndTrans.Value = 0;
            $scope.f.ReimAirOtherVal.Value = 0;

            $scope.f.AirFareAmount.Value = 0;
            $scope.f.HotelLodAmount.Value = 0;
            $scope.f.MealsAmount.Value = 0;
            $scope.f.RegisFeeAmount.Value = 0;
            $scope.f.GroundTransAmount.Value = 0;
            $scope.f.OtherAmount.Value = 0;

            $scope.f.Payment.Value = 0;
            $scope.f.Allowance.Value = 0;    
        }
        else{
            if($scope.f.AirFareDate.Value == null || $scope.f.AirFareDate.Value.toString().indexOf('nvalid') != -1 ){
                $scope.f.AirFareDate.Value = '';
            }
            if($scope.f.HotelLodDate.Value == null || $scope.f.HotelLodDate.Value.toString().indexOf('nvalid') != -1){
                $scope.f.HotelLodDate.Value = '';
            }
            if($scope.f.MealsDate.Value == null || $scope.f.MealsDate.Value.toString().indexOf('nvalid') != -1){
                $scope.f.MealsDate.Value = '';
            }
            if($scope.f.RegisFeeDate.Value == null || $scope.f.RegisFeeDate.Value.toString().indexOf('nvalid') != -1){
                $scope.f.RegisFeeDate.Value = '';
            }
            if($scope.f.GroundTransDate.Value == null || $scope.f.GroundTransDate.Value.toString().indexOf('nvalid') != -1){
                $scope.f.GroundTransDate.Value = '';
            }
            if($scope.f.OtherDate.Value == null || $scope.f.OtherDate.Value.toString().indexOf('nvalid') != -1){
                $scope.f.OtherDate.Value = '';
            }
        }
    }

    function populateCurrentUserAsArequestor(){
        if($scope.formMode == 'New'){
                    //Select current user as a Requestor by default
                    var getUserRestURL = _spPageContextInfo.webServerRelativeUrl + "/_api/Web/GetUserById(" + _spPageContextInfo.userId + ")";
                    $http.get(getUserRestURL).then(function(requestResponse) {
                            $scope.f.Traveler.user = {
                                Name: requestResponse.data.d.LoginName,
                                Id: requestResponse.data.d.Id, 
                                Title:requestResponse.data.d.Title
                            }
                    });
                }
    }

});