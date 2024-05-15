using System.Collections.Generic;
using System.Threading.Tasks;
using HubSpot.NET.Api.ContactList;
using HubSpot.NET.Api.ContactList.Dto;

namespace HubSpot.NET.Core.Interfaces;

public interface IHubSpotContactListApi
{
    ListContactListModel GetContactLists(ListOptions opts = null);
    Task<ListContactListModel> GetContactListsAsync(ListOptions opts = null);

    ListContactListModel GetStaticContactLists(ListOptions opts = null);
    Task<ListContactListModel> GetStaticContactListsAsync(ListOptions opts = null);

    ContactListModel GetContactListById(long contactListId);
    Task<ContactListModel> GetContactListByIdAsync(long contactListId);

    ContactListUpdateResponseModel AddContactsToList(long listId, IEnumerable<long> contactIds);
    Task<ContactListUpdateResponseModel> AddContactsToListAsync(long listId, IEnumerable<long> contactIds);

    ContactListUpdateResponseModel RemoveContactsFromList(long listId, IEnumerable<long> contactIds);
    Task<ContactListUpdateResponseModel> RemoveContactsFromListAsync(long listId, IEnumerable<long> contactIds);

    void DeleteContactList(long listId);
    Task DeleteContactListAsync(long listId);

    ContactListModel CreateStaticContactList(string contactListName);
    Task<ContactListModel> CreateStaticContactListAsync(string contactListName);
}