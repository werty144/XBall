package com.example.game

import kotlinx.serialization.Serializable
import kotlin.math.*

@Serializable
data class Point(val x: Float, val y: Float) {
    operator fun plus(vector: Vector): Point {
        return Point(x + vector.x, y + vector.y)
    }
}

fun distance(p1: Point, p2: Point) = Vector(p1, p2).length()

@Serializable
class Vector{
    val x: Float
    val y: Float

    constructor(x: Float, y: Float) { this.x =  x; this.y = y }

    constructor(p1: Point, p2: Point) {
        val xDiff = p2.x - p1.x
        val yDiff = p2.y - p1.y
        this.x = xDiff
        this.y = yDiff
    }

    fun unit(): Vector = Vector(x / length(), y / length())

    operator fun times(n: Float): Vector = Vector(x * n, y * n)

    operator fun plus(other: Vector): Vector = Vector(x + other.x, y + other.y)

    fun length(): Float = sqrt(x * x + y * y)

    fun angle(): Float = atan2(y, x)

    fun rotated(angle: Float): Vector = Vector(x * cos(angle) - y * sin(angle), x * sin(angle) + y * cos(angle))

    fun orientedAngleWithVector(v: Vector): Float = atan2(x * v.y - y * v.x, x * v.x + y * v.y)

    fun angleWithVector(v: Vector): Float = abs(orientedAngleWithVector(v))

    fun orthogonalUnit(): Vector = Vector(-y, x).unit()

}