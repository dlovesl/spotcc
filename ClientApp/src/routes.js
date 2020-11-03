import Vue from 'vue'
import VueRouter from 'vue-router'
import Pre from './components/Pre.vue'
import Free from './components/Free.vue'
import FrameworksDownloads from "./components/FrameworksDownloads";
import ArtistChart from './components/ArtistChart.vue'

Vue.use(VueRouter)

export const router = new VueRouter({
    routes: [
        {path: '/', component: FrameworksDownloads},
        {path: '/pre', component: Pre},
        {path: '/free', component: Free},
        {path: '/chart', component: ArtistChart}
    ]
})