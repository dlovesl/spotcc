import Vue from "vue";
import App from "./App.vue";

import axios from "axios";
import VueAxios from "vue-axios";
import TrendChart from "vue-trend-chart";
import { router } from './routes'
import vSelect from "vue-select";
import Buefy from 'buefy'
import 'buefy/dist/buefy.css'

Vue.use(Buefy)
Vue.use(VueAxios, axios);
Vue.use(TrendChart);
Vue.use(vSelect);
Vue.component('v-select', vSelect)
Vue.config.productionTip = false;

new Vue({
  router,
  render: (h) => h(App),
}).$mount("#app");
