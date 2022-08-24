package com.xballserver.remoteserver.infrastructure

import com.xballserver.remoteserver.game.Game
import com.xballserver.remoteserver.game.Side
import com.xballserver.remoteserver.routing.createCancelGameJSONString
import com.xballserver.remoteserver.routing.createPrepareGameJSONString
import com.xballserver.remoteserver.routing.createStartGameJSONString
import kotlinx.coroutines.*
import java.sql.Timestamp
import java.util.*
import kotlin.collections.LinkedHashSet

class GameStartManager
    (
        private val gamesManager: GamesManager,
        private val lobbyManager: LobbyManager,
        private val connectionManager: ConnectionManager
    ) {

    val gameReadyEntities: MutableSet<GameReadyEntity> = Collections.synchronizedSet(LinkedHashSet())
    private val gameReadyEntitiesManageScope: CoroutineScope = CoroutineScope(CoroutineName("Game ready management"))

    init {
        gameReadyEntitiesManageScope.launch {
            while (true) {
                delay(1000L)
                manageGameReadyEntities()
            }
        }
    }

    suspend fun handleLobbyReadyRequest(lobby: Lobby, userId: UserId) {
        if (!lobbyManager.exists(lobby.id)) {
            lobbyManager.createLobby(lobby.id, lobby.maxCapacity, lobby.gameProperties)
        }

        lobbyManager.addUserToLobby(lobby.id, userId)

        if (lobbyManager.allReady(lobby.id)) {
            prepareGame(lobby.id)
        }
    }

    suspend fun prepareGame(lobbyID: LobbyID) {
        val lobby = lobbyManager.getLobby(lobbyID) ?: return
        val game = gamesManager.createGameFromLobby(lobby) ?: return
        sendPrepareGameMessages(game)
        gameReadyEntities.add(GameReadyEntity(game, lobby.maxCapacity))
    }

    suspend fun sendPrepareGameMessages(game: Game) {
        val firstUserMessage = createPrepareGameJSONString(game, Side.LEFT)
        val secondUserMessage = createPrepareGameJSONString(game, Side.RIGHT)
        coroutineScope {
            launch { connectionManager.sendMessage(game.user1Id, firstUserMessage) }
            launch { connectionManager.sendMessage(game.user2Id, secondUserMessage) }
        }
    }

    suspend fun manageGameReadyEntities() {
        gameReadyEntities.forEach {
            if (it.isOutDated()) {
                val message = createCancelGameJSONString()
                connectionManager.sendMessage(it.game.user1Id, message)
                connectionManager.sendMessage(it.game.user2Id, message)
            }
        }
        gameReadyEntities.removeIf { it.isOutDated() }
    }

    suspend fun handleGameReadyRequest(userId: UserId) {
        val gameReadyEntity = gameReadyEntities.find { (it.game.user1Id == userId) or (it.game.user2Id == userId) } ?: return
        gameReadyEntity.members.add(userId)
        if (gameReadyEntity.members.size == gameReadyEntity.nUsers) {
            sendStartGameMessages(gameReadyEntity.game)
            gamesManager.startGame(gameReadyEntity.game)
            gameReadyEntities.remove(gameReadyEntity)
        }
    }

    suspend fun sendStartGameMessages(game: Game) {
        val message = createStartGameJSONString()
        coroutineScope {
            launch { connectionManager.sendMessage(game.user1Id, message) }
            launch { connectionManager.sendMessage(game.user2Id, message) }
        }
    }
}

class GameReadyEntity
(
    val game: Game,
    val nUsers: Int,
    val members: MutableSet<UserId> = Collections.synchronizedSet(LinkedHashSet()),
    val timeStamp: Timestamp = Timestamp(System.currentTimeMillis()),
    val outDateTime: Long = 10_000L
) {
    fun isOutDated(): Boolean {
        val currentTimeStamp = Timestamp(System.currentTimeMillis())
        return currentTimeStamp.time - timeStamp.time > outDateTime
    }
}