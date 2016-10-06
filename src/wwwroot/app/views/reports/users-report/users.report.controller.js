import {
   set,
   remove,
   assign,
   each,
   isEmpty
} from 'lodash';
const LIST_OF_LOCATIONS = ['Dnipropetrovsk', 'Zaporizhia', 'Lviv', 'Berdyansk'];

export default function UsersReportController(
   $scope,
   UserService,
   ThesaurusService,
   ReportsService
) {
   'ngInject';

   const vm                                = $scope;
   vm.users                                = [];
   vm.usersReportParametrs                 = {};
   vm.usersReportParametrs.locationIds     = [];
   vm.usersReportParametrs.userIds         = [];
   vm.locations                            = [];
   vm.selectedLocations                    = [];
   vm.selectedUsers                        = [];
   vm.selectedUsersGroupedByLocation       = {};
   vm.clear                                = clear;
   vm.useLocationField                     = useLocationField;
   vm.useUserField                         = useUserField;
   vm.addLocationIdsToUsersReportParametrs = addLocationIdsToUsersReportParametrs;
   vm.addUserIdsToUsersReportParametrs     = addUserIdsToUsersReportParametrs;
   vm.formingUsersReport                   = formingUsersReport;
   vm.isEqualLocations                     = isEqualLocations;
   vm.isSelectedUsersGroupedByLocationEmpty = isSelectedUsersGroupedByLocationEmpty;

   (function init() {
      ThesaurusService.getThesaurusTopics('stage').then(topic => set(vm, 'stages', topic));
      ThesaurusService.getThesaurusTopics('city').then(locations => {
         each(locations, (location) => {
            if (LIST_OF_LOCATIONS.includes(location.title)) {
               vm.locations.push(location);
            }
            if (vm.usersReportParametrs.locationIds.length &&
                vm.usersReportParametrs.locationIds.includes(location.id)) {
               assign(location, {selected: true});
            } else {
               assign(location, {selected: false});
            }
         });
      });
      UserService.getUsers().then(users => {
         set(vm, 'users', users);
         each(vm.users, (user) => {
            if (vm.usersReportParametrs.userIds.length &&
                vm.usersReportParametrs.userIds.includes(user.id)) {
               assign(user, {selected: true});
            } else {
               assign(user, {selected: false});
            }
         });
      });
   }());

   function useLocationField(isActiveLocationField) {
      if (!isActiveLocationField) {
         _clearLocationField();
      }
   }

   function useUserField(isActiveUsersField) {
      if (!isActiveUsersField) {
         _clearUserField();
      }
   }

   function _clearLocationField() {
      vm.usersReportParametrs.locationIds = [];
      vm.selectedLocations = [];
      vm.selectedUsersGroupedByLocation = {};
      each(vm.locations, (location) => {
         location.selected = false;
      });
   }

   function _clearUserField() {
      vm.usersReportParametrs.userIds = [];
      vm.selectedUsers = [];
      vm.selectedUsersGroupedByLocation = {};
      each(vm.users, (user) => {
         user.selected = false;
      });
   }

   function addLocationIdsToUsersReportParametrs(location) {
      if (location.selected && !vm.usersReportParametrs.locationIds.includes(location.id)) {
         vm.usersReportParametrs.locationIds.push(location.id);
         vm.selectedLocations.push(location);
      } else if (!location.selected) {
         remove(vm.usersReportParametrs.locationIds, (locationId) =>  locationId === location.id);
         remove(vm.selectedLocations, (loc) =>  loc.id === location.id);
      }
   }

   function addUserIdsToUsersReportParametrs(user) {
      if (user.selected && !vm.usersReportParametrs.userIds.includes(user.id)) {
         vm.usersReportParametrs.userIds.push(user.id);
         if (vm.selectedLocations.length) {
            _convertUsersArrayToHash(user);
         } else {
            vm.selectedUsers.push(user);
         }
      } else if (!user.selected) {
         remove(vm.usersReportParametrs.userIds, (userId) =>  userId === user.id);
         if (vm.selectedLocations.length) {
            remove(vm.selectedUsersGroupedByLocation[user.cityId], {id: user.id});
         } else {
            remove(vm.selectedUsers, (us) =>  us.id === user.id);
         }
      }
   }

   function formingUsersReport() {
      ReportsService.getDataForUserReport(vm.usersReportParametrs).then(resp => {
         vm.usersReportParametrs.startDate = resp.startDate;
         vm.usersReportParametrs.endDate = resp.endDate;
//            _convertReportToHash(resp);
      });;
   }

   function clear() {
      vm.usersReportParametrs = {};
      _clearLocationField();
      _clearUserField();
   }

   function isEqualLocations(user) {
      if (vm.usersReportParametrs.locationIds.length) {
         return vm.usersReportParametrs.locationIds.includes(user.cityId);
      } else {
         return true;
      }
   }

   function isSelectedUsersGroupedByLocationEmpty() {
      return isEmpty(vm.selectedUsersGroupedByLocation);
   }

   function _convertUsersArrayToHash(user) {
      each(vm.selectedLocations, location => {
         if (vm.selectedUsersGroupedByLocation[location.id] && location.id === user.cityId) {
            vm.selectedUsersGroupedByLocation[location.id].push(user);
         } else if (!vm.selectedUsers[location.id] && location.id === user.cityId) {
            vm.selectedUsersGroupedByLocation[location.id] = [];
            vm.selectedUsersGroupedByLocation[location.id].push(user);
         }
      });
   }

}
