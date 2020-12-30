import Vue from 'vue'
import VueRouter from 'vue-router'
import Login from './components/Login.vue'
import Pre from './components/Pre.vue'
import AddEditArtist from './components/AddEditArtist.vue'
import ArtistDownloadChart from './components/ArtistDownloadChart.vue'

Vue.use(VueRouter)

export const router = new VueRouter({
    routes: [
        {path: '/pre', component: Pre},
        {path: '/artist', component: AddEditArtist},
        {path: '/stats', component: ArtistDownloadChart},
        {path: '/', component: ArtistDownloadChart, name: 'home'},
        {path: '/login', component: Login}
    ]
})