"use strict";
app.controller('MainCtrl', function ($scope, $http, $log, $location,spFormFactory) {
    spFormFactory.initialize($scope, 'Sample List').then(init);
    $scope.calendarFormat = 'dd-MMMM-yyyy';
    $scope.userId = _spPageContextInfo.userId;

    function init(){
       //Add your logic, scope variables, functions HERE:
       //$scope.SomeVariable = 'example'
       //$scope.f.Title.Value = 'Default Title vaue';
    }
});