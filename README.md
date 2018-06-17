# Automatically Generates AngularJs Form for any SharePoint list
Custom SharePoint Forms for SharePoint 2013/2016/SPOnline. Using AngularJS

Angular View and Controller are automatically generated and applied to the target SharePoint list of your choice.

![Form Sample](https://github.com/Zerg00s/AngularForms/blob/master/FormSample.jpg?raw=true)

## How to Deploy Angular Forms:
- Download the project
- Open it with Visual Studio
- Specify site URL, your login and a target list title that will have Angular forms applied to it
- Hit F5 to deploy. Angular view and controller will be automatically generated depending on the current list fields

![Form Sample](https://github.com/Zerg00s/AngularForms/blob/master/Deploy.jpg?raw=true)


## How to start development:
Map the /Assets/App folder using WebDav. First Open it in Internet Explorer for authentication purposes. Open it in Visual Studio Code.
Angular views and controllers will be deployed in the /Assets/App/Forms/Your_List_Name/ folder.

You will have to know your field internal names to be able to refer them.
From the controller you refer to the fields like so: $scope.f.fieldName.Value. 
From the View you refer to the fields like so: {{f.fieldName.Value}} or ng-model="f.fieldName.Value" 

The generated Angular form is just a scaffolding and is meant to be modified according to your business logic. You will probably want to add ng-required, ng-hidden, et. to your fields

Saving and loading happens automatically. Not all field types are available. Refer to the bottom section for the full list.

Missing field types:
- multichoice
- lookup/multilookup
- taxonomy 

Feel free to contribute to the Angular service spFormFactory.js to include support for the missing field types

## Available field types: 
```
<!-- user field -->
<div class="col-md-4 col-xs-6">
    <h4>{{f.Requestor.FieldDisplayName}}</h4>
    <div ui-people ng-model="f.Requestor.user" pp-is-multiuser="{{false}}" pp-width="220px" pp-account-type="User" id="Requestor"></div>
</div>

<!-- date -->
<div class="col-md-4 col-xs-6">
    <h4>{{f.StartDate.FieldDisplayName}}</h4>
    <datetime-picker format="calendarFormat" ng-model="f.StartDate.Value" name="StartDate" id="StartDate" />
</div>

<!-- time -->
<div class="col-md-3 col-xs-6">
    <h4>Time</h4>
    <span class='time-span'>
        <b>{{f.StartTime.FieldDisplayName}}</b>
    </span>
    <div uib-timepicker ng-change='updateTime()' ng-model="f.StartTime.Value" id="StartTime" name="StartTime" show-spinners='false'></div>
</div>

<!-- single choice checkbox -->
<div class="col-md-4 col-xs-6">
    <h4>Duration</h4>
    <choice-field field="f.choiceField" id="choiceField"> </choice-field>
</div>

<!-- radio -->
<div class="col-md-3 col-xs-6">
    <h4>{{f.WithPay.FieldDisplayName}}</h4>
    <radio-field field="f.WithPay" id="WithPay" ng-required="true"> </radio-field>
</div>

<!-- dropdown -->
<div class="col-md-3">
    <h4>Option</h4>
    <select ng-required="true" data-ng-model="f.OptionField.Value" name="OptionField" class="selectpicker">
        <option ng-repeat="option in f.OptionField.Choices">{{option}}</option>
    </select>
</div>

<!-- number -->
<input type="number" ng-model="f.NumberOfDays.Value" min="0" id="NumberOfDays" />

<!-- currency -->
<div class='col-md-4 col-xs-6'>
    <h4>Price</h4>
    <input type='text' maxlength='254' id='Price' name='Price' ng-model='f.Price.Value' class='full-width' ng-currency  placeholder="insert currency value"  />
</div>

<!-- single line text -->
<div class="col-md-3 col-xs-4">
    <h4>{{f.PhoneNumber.FieldDisplayName}}</h4>
    <input type='text' maxlength='254' id='PhoneNumber' name='PhoneNumber' ng-model="f.PhoneNumber.Value" class='full-width' />
</div>

<!-- multiline text -->
<div class="col-md-6">
    <h4>{{f.Comments.FieldDisplayName}}</h4>
    <textarea ng-model="f.Comments.Value" name="Comments"></textarea>
</div>

<!-- attachments -->
<div class="col-md-12 non-printable" ng-if='CurrentUserPartOfWorkflow'>
    <h4>Documents:</h4>
    <file-uploader control-id="'Documents'"
                   attachment-filter-field-name="'DocumentsID'"
                   show-display-name-field="false" attachment-doc-folder-name="'Documents'" />
    <!-- specify control-id if you have more than 1 attachment control on the form -->
    <!-- attachment-filter-field-name: internal lookup field name that refers to the current list. Create it in the Attachments Documents if it does not exist.-->
    <!-- show-display-name-field: true if you want to rename the files-->
    <!-- attachment-doc-folder-name: name of the folder in the Attachments library where you want to store the documents. default is '' - root -->
</div>
```
