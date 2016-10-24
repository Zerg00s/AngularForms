"use strict";
app.controller('MainCtrl', function ($scope, $http, $log, $location, spFormFactory) {    
    $scope.spFormFactory = spFormFactory;

    spFormFactory.initialize($scope, 'LIST_TITLE').then(init);
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
   
    }

});