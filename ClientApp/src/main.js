import Vue from "vue";
import App from "./App.vue";

import axios from "axios";
import VueAxios from "vue-axios";
import TrendChart from "vue-trend-chart";
import { router } from './routes'

Vue.use(VueAxios, axios);
Vue.use(TrendChart);

Vue.config.productionTip = false;

new Vue({
  router,
  render: (h) => h(App),
}).$mount("#app");
