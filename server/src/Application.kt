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

    val invitesManager = InvitesManager()
    val gamesManager = GamesManager()
    val authenticationManager = AuthenticationManager()
    val connections = Collections.synchronizedSet<Connection?>(LinkedHashSet())

    configureRouting(gamesManager, invitesManager, authenticationManager, connections)

    val logger = Logger(invitesManager, gamesManager, connections)
    launch { logger.logPeriodically() }

    val cleaner = Cleaner(invitesManager, gamesManager, connections)
    launch { cleaner.cleanPeriodically() }
}
