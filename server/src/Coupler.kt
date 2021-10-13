package com.example

typealias UserId = Int
typealias InviteId = Int

data class Invite(val inviteId: InviteId, val inviterId: UserId, val invitedId: UserId)

class Coupler {
    private var spareUserId: UserId = 0
    private var spareInviteId: InviteId = 0
    private val invites = ArrayList<Invite>()

    fun getNewUserId() = spareUserId++;

    fun formNewInvite(inviterId: UserId, invitedId: UserId) {
        if (invites.any {(it.inviterId == inviterId) and (it.invitedId == invitedId)}) return

        invites.add(Invite(spareInviteId++, inviterId, invitedId))
    }

    fun getInvitesForUser(userId: UserId): List<Invite> {
        return invites.filter {it.invitedId == userId}
    }

    fun getInviterId(userId: UserId, inviteId: InviteId): UserId? {
        if (! invites.any { (it.inviteId == inviteId) and (it.invitedId == userId) }) return null
        return invites.first { (it.inviteId == inviteId) and (it.invitedId == userId) }.inviterId
    }
}