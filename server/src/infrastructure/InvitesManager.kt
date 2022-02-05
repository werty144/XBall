package com.example.infrastructure

import com.example.game.GameProperties
import com.example.routing.APIInvite
import kotlinx.serialization.Serializable
import java.sql.Timestamp

typealias UserId = Int
typealias InviteId = Int

@Serializable
data class Invite(val inviteId: InviteId,
                  val inviterId: UserId,
                  val invitedId: UserId,
                  val gameProperties: GameProperties,
                  @kotlinx.serialization.Transient
                  val timeStamp: Timestamp = Timestamp(System.currentTimeMillis())
) {
    override fun equals(other: Any?): Boolean {
        return if (other is Invite) {
            (inviterId == other.inviterId) and (invitedId == other.invitedId) and (gameProperties == other.gameProperties)
        } else {
            false
        }
    }

    override fun hashCode(): Int {
        return inviteId
    }
}

class InvitesManager {
    private var spareUserId: UserId = 0
    private var spareInviteId: InviteId = 0
    val invites = ArrayList<Invite>()

    fun getNewUserId() = spareUserId++

    fun validateAPIInvite(apiInvite: APIInvite): Boolean {
        return listOf(1, 2, 3).contains(apiInvite.playersNumber)
    }

    fun formNewInvite(inviterId: UserId, apiInvite: APIInvite): Invite? {
        val newInvite = Invite(
            spareInviteId++,
            inviterId,
            apiInvite.invitedId,
            GameProperties(apiInvite.playersNumber, apiInvite.speed)

        )

        if ((invites.any { it == newInvite }) or !validateAPIInvite(apiInvite)) return null

        invites.add(newInvite)
        return newInvite
    }

    fun getInviteById(inviteId: InviteId): Invite? {
        return invites.firstOrNull { it.inviteId == inviteId }
    }

    fun removeInviteById(inviteId: InviteId) = invites.removeIf { it.inviteId == inviteId }

    fun getInvitesForUser(userId: UserId): List<Invite> {
        return invites.filter {it.invitedId == userId}
    }

    fun getInviterId(userId: UserId, inviteId: InviteId): UserId? {
        if (! invites.any { (it.inviteId == inviteId) and (it.invitedId == userId) }) return null
        return invites.first { (it.inviteId == inviteId) and (it.invitedId == userId) }.inviterId
    }
}