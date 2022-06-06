package com.example

import com.example.infrastructure.*
import com.example.routing.Connection
import com.example.routing.configureRouting
import io.ktor.application.*
import io.ktor.features.*
import io.ktor.serialization.*
import io.ktor.websocket.*
import kotlinx.coroutines.launch
import java.util.*
import kotlin.collections.LinkedHashSet

fun main(args: Array<String>): Unit = io.ktor.server.netty.EngineMain.main(args)


@Suppress("unused") // Referenced in application.conf
@kotlin.jvm.JvmOverloads
fun Application.module(testing: Boolean = false) {
    install(WebSockets)
    install(ContentNegotiation) {
        json()
    }

    val connections = Collections.synchronizedSet<Connection?>(LinkedHashSet())
    val gamesManager = GamesManager(connections)
    val lobbyManager = LobbyManager(gamesManager)
    val authenticationManager = AuthenticationManager()

    configureRouting(gamesManager, lobbyManager, authenticationManager, connections)

    val logger = Logger(lobbyManager, gamesManager, connections)
    launch { logger.logPeriodically() }

    val cleaner = Cleaner(lobbyManager, gamesManager, connections)
    launch { cleaner.cleanPeriodically() }
}
