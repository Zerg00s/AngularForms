//App Controller
(function () {
    'use strict';
    var controllerId = 'appCtrlr';
    angular.module('app').controller(controllerId, ['$scope', '$timeout', appCtrlr]);

    function appCtrlr($scope, $timeout) {
		var vm = this;
		vm.webUrl = null;
		vm.data = {
			su:null,
			mu:null
		};			
		vm.getPresence = getPresence;
		var service = function () {		
			this.webAbsoluteUrl = _spPageContextInfo.webAbsoluteUrl;
			this.webRelativeUrl = _spPageContextInfo.webServerRelativeUrl;
			this.currentUserId = _spPageContextInfo.userId;
			this.webAppUrl = document.location.origin || document.location.href.substring(0, document.location.href.indexOf(document.location.pathname));		
		}
         
		init();

		function init() {
			//Normally you would get this information from a REST call to Office 365 / SharePoint
			getData();	
			//Update the weburl property
			//This is to demo passing in a Web URL to the people picker via the pp-web-url attribute.
			$timeout(function(){
				var s = new service;
				vm.webUrl = s.webAppUrl;
				if (!$scope.$root.$$phase) {
					$scope.$apply();
				}				 
			 },500);
		};
		
		function getData(){
			$timeout(function(){
				vm.data.su = {
					//You'll need to add a valid claims based identity, and the SPUSER ID here
					Name:'i:0#.f|membership|dmolodtsov@jolera.com',
					Id:'22', 
					Title:'Denis Molodtsov'
					};
				//Add a user to the mulitple-user model
				vm.data.mu = {results:[]};				
				vm.data.mu.results.push({
					//You'll need to add a valid claims based identity, and the SPUSER ID here
					Name:'i:0#.f|membership|dmolodtsov@jolera.com',
					Id:'22', 
					Title:'Denis Molodtsov'});
				//Add a SharePoint group to the mulitple-user model
				vm.data.mu.results.push({
				//You'll need to add a valid claims based identity, and the SPUSER ID here
					Name:'App Center Owners',
					Id:'3', 
					Title:'App Center Owners'});
				if (!$scope.$root.$$phase) {
					$scope.$apply();
				}
			},100)
			
		}
               
		function getPresence(userId, userTitle) {
			if (userId && userTitle) {
				return '<span class="ms-noWrap"><span class="ms-spimn-presenceLink"><span class="ms-spimn-presenceWrapper ms-imnImg ms-spimn-imgSize-10x10"><img class="ms-spimn-img ms-spimn-presence-disconnected-10x10x32" src="'+service.webAbsoluteUrl+'/_layouts/15/images/spimn.png?rev=23"  alt="" /></span></span><span class="ms-noWrap ms-imnSpan"><span class="ms-spimn-presenceLink"><img class="ms-hide" src="'+service.webAbsoluteUrl+'/_layouts/15/images/blank.gif?rev=23"  alt="" /></span><a class="ms-subtleLink" onclick="GoToLinkOrDialogNewWindow(this);return false;" href="'+service.webAbsoluteUrl+'/_layouts/15/userdisp.aspx?ID=' + userId + '">' + userTitle + '</a></span></span>';
			}
			return '<span></span>';
		}
	}
})();
