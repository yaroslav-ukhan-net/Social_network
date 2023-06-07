using Models.Models;
using Services;
using Social_network.ViewModels;
using SocialNetwork.ViewModels;
using System.Collections.Generic;

namespace SocialNetwork.CheckingAccess
{
    public static class CheckingAccess
    {
        public static int CheckFriendship(int myUserId, int id, FriendService _friendService)
        {
            UserViewModel userViewModel = new UserViewModel();
            if (myUserId != id)  //friendship status
            {
                var frienshipStatus = _friendService.GetFriendsByTwoId(myUserId, id);
                if (frienshipStatus != null)
                {
                    userViewModel.friendshipStatus = frienshipStatus.Status;
                    if (frienshipStatus.Status == (int)StatusFriendship.requestToFriendship) userViewModel.friendshipStatus = (int)friendshipStatusEnum.requestForUser;
                }
                else
                {
                    frienshipStatus = _friendService.GetFriendsByTwoId(id, myUserId);
                    if (frienshipStatus != null)
                    {
                        userViewModel.friendshipStatus = frienshipStatus.Status;
                        if (frienshipStatus.Status == (int)StatusFriendship.requestToFriendship) userViewModel.friendshipStatus = (int)friendshipStatusEnum.requestFromUser;
                    }
                    else
                    {
                        userViewModel.friendshipStatus = (int)friendshipStatusEnum.notFriends;

                    }
                }
            }
            else
            {
                return (int)friendshipStatusEnum.itsMyPage;
            }
            return userViewModel.friendshipStatus;
        }

        public static PageAccess CheckAccess(int myUserId, int id, Models.User  userPage, bool friendship)
        {
            PageAccess access = new PageAccess();
            access.AccessSeeMyPhone = CheckOneProperty(myUserId, id, userPage.Settings.SeeMyPhone, friendship);
            access.AccessChatInvites = CheckOneProperty(myUserId, id, userPage.Settings.ChatInvites, friendship); //-
            access.AccessSeeMyGroups = CheckOneProperty(myUserId, id, userPage.Settings.SeeMyGroups, friendship); //-
            access.AccessWriteToMe = CheckOneProperty(myUserId, id, userPage.Settings.WriteToMe, friendship); //-
            access.AccessLeavePosts = CheckOneProperty(myUserId, id, userPage.Settings.LeavePosts, friendship); //-
            access.AccessSeeMyPosts = CheckOneProperty(myUserId, id, userPage.Settings.SeeMyPosts, friendship);
            access.AccessSeeMyFriends = CheckOneProperty(myUserId, id, userPage.Settings.SeeMyFriends, friendship);

            return access;
        }
        public static bool CheckOneProperty(int id1, int id2, int userSetting, bool friendship)
        {
            if (id1 == id2) return true;                            // my page
            else if (userSetting == 2) return true;                 // all have access
            else if (userSetting == 1 && friendship) return true;   // for friends
            return false;
        }

    }
}
