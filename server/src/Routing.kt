package com.example

import io.ktor.application.*
import io.ktor.http.cio.websocket.*
import io.ktor.routing.*
import io.ktor.websocket.*
import java.util.*
import java.util.concurrent.atomic.AtomicInteger
import kotlin.collections.LinkedHashSet


fun Application.configureRouting() {

    val apiHandler = APIHandler(this)

    routing {
        val connections = Collections.synchronizedSet<Connection?>(LinkedHashSet())


        webSocket("/") {
            val thisConnection = Connection(this)
            connections += thisConnection
            send("${thisConnection.id}")
            for (frame in incoming) {
                apiHandler.handle(frame, thisConnection, connections)
            }
        }
    }
}

class Connection(val session: DefaultWebSocketSession) {
    companion object {
        var lastId = AtomicInteger(0)
    }

    val id = lastId.getAndIncrement()
}