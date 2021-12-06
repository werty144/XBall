package com.example.infrastructure

import com.example.game.GameProperties
import com.example.routing.APIInvite
import kotlinx.serialization.Serializable

typealias UserId = Int
typealias InviteId = Int

@Serializable
data class Invite(val inviteId: InviteId, val inviterId: UserId, val invitedId: UserId, val gameProperties: GameProperties)

class Coupler {
    private var spareUserId: UserId = 0
    private var spareInviteId: InviteId = 0
    private val invites = ArrayList<Invite>()

    fun getNewUserId() = spareUserId++;

    fun formNewInvite(inviterId: UserId, apiInvite: APIInvite): Invite? {
        val invitePredicate = {it: Invite -> (it.inviterId == inviterId) and (it.invitedId == apiInvite.invitedId)}

        if (invites.any { invitePredicate(it) }) return null

        val newInvite = Invite(spareInviteId++, inviterId, apiInvite.invitedId, GameProperties(apiInvite.playersNumber, apiInvite.speed))
        invites.add(newInvite)
        return newInvite
    }

    fun getInviteById(inviteId: InviteId): Invite {
        return invites.first { it.inviteId == inviteId }
    }

    fun getInvitesForUser(userId: UserId): List<Invite> {
        return invites.filter {it.invitedId == userId}
    }

    fun getInviterId(userId: UserId, inviteId: InviteId): UserId? {
        if (! invites.any { (it.inviteId == inviteId) and (it.invitedId == userId) }) return null
        return invites.first { (it.inviteId == inviteId) and (it.invitedId == userId) }.inviterId
    }
}