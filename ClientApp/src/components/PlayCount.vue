<template>
  <div v-if="dataLoaded" class="vue-poll">
    <div class="ans-cnt">
      <div
        v-for="(acc, index) in calcStreamPercentages"
        :key="index"
        class="ans"
      >
        <template>
          <div class="ans">
            <div class="ans-voted">
              <span
                class="txt"
                style="margin: 0px 0px 10px 10px"
                v-text="acc.name + ' ('"
              ></span>
              <span
                class="percent"
                style="margin: 0px; min-width: 0px"
                v-text="acc.percent"
              ></span>
              <span
                class="txt"
                v-text="
                  ') - Total Stream: ' +
                  formatPrice(acc.totalPlays) +
                  '/' +
                  formatPrice(acc.totalStreams)
                "
              ></span>
              <span :class="{ bg: true }" style="width: 100%"></span>
              <span
                :class="{ bg: true, selected: true }"
                :style="{ width: acc.percent }"
              ></span>
            </div>
          </div>
          <div
            v-for="(a, i) in acc.artists"
            :key="i"
            class="ans"
            style="margin-left: 10px"
          >
            <template>
              <div class="ans-voted final">
                <span class="percent" v-html="'Played: ' + a.plays"></span>
                <span class="txt" v-html="a.name"></span>
                <span
                  class="txt"
                  v-html="' - Listeners: ' + a.monthlyListeners"
                ></span>
                <span
                  class="txt"
                  v-html="' - Followers: ' + a.followers"
                ></span>
              </div>
              <span :class="{ bg: true }" style="width: 100%"></span>
            </template>
          </div>
        </template>
      </div>
    </div>
  </div>
</template>

<script>
import axios from 'axios';
import Api from "../services/api";

export default {
  name: "PlayCount",
  data() {
    return {
      accountsInfo: [],
      dataLoaded: false,
    };
  },
  props: {
    accounts: {
      type: Array,
      required: true,
    },
  },
  computed: {
    calcStreamPercentages() {
      return this.accountsInfo.map((acc) => {
        if (isNaN(acc.totalStreams) || acc.totalStreams === 0) {
          acc.percent = "0%";
          acc.totalPlays = 0;
          return acc;
        }
        let totalPlays = acc.artists
          .map((a) => a.plays)
          .reduce((totalPlays, plays) => {
            if (!isNaN(plays) && plays > 0) return totalPlays + plays;
            else return totalPlays;
          });
        if (totalPlays > 0)
          acc.percent =
            Math.round((parseInt(totalPlays) / acc.totalStreams) * 100) + "%";
        else acc.percent = "0%";
        acc.totalPlays = totalPlays;
        return acc;
      });
    },
  },
  created() {
    let t = this.accounts.map(async (a) => {
      let info = {};
      info.name = a.name;
      info.totalStreams = a.totalStreams;
      let artistInfoRequests = a.artists.map((artistId) => {
        return Api().get(`/artist/spotify/` + artistId);
      });

      return axios.all(artistInfoRequests).then(
        axios.spread((...responses) => {
          info.artists = responses.map((r) => {
            let artistInfo = {};
            let totalPlays = 0;
            artistInfo.name = r.data.name;
            artistInfo.monthlyListeners = r.data.monthlyListeners;
            artistInfo.followers = r.data.followerCount;
            totalPlays += r.data.totalStreams;
            artistInfo.plays = totalPlays;
            return artistInfo;
          });

          return info;
        })
      );
    });

    Promise.all(t).then((data) => {
      this.accountsInfo = data;
      this.dataLoaded = true;
    });
  },
  methods: {
    formatPrice(value) {
      let val = (value / 1).toFixed(0).replace(".", ",");
      return val.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    },
  },
};
</script>

<style>
.vue-poll {
  font-family: "Avenir", Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  color: #2c3e50;
}

.vue-poll .noselect {
  -webkit-touch-callout: none;
  -webkit-user-select: none;
  -khtml-user-select: none;
  -moz-user-select: none;
  -ms-user-select: none;
  user-select: none;
}

.vue-poll .qst {
  font-weight: normal;
}
.vue-poll .ans-cnt {
  margin: 20px 0;
}
.vue-poll .ans-cnt .ans {
  position: relative;
  margin-top: 10px;
}
.vue-poll .ans-cnt .ans:first-child {
  margin-top: 0;
}

.vue-poll .ans-cnt .ans-no-vote {
  text-align: center;
  border: 2px solid #77c7f7;
  box-sizing: border-box;
  border-radius: 5px;
  cursor: pointer;
  padding: 5px 0;
  transition: background 0.2s ease-in-out;
  -webkit-transition: background 0.2s ease-in-out;
  -moz-transition: background 0.2s ease-in-out;
}

.vue-poll .ans-cnt .ans-no-vote .txt {
  color: #77c7f7;
  transition: color 0.2s ease-in-out;
  -webkit-transition: color 0.2s ease-in-out;
  -moz-transition: color 0.2s ease-in-out;
}

.vue-poll .ans-cnt .ans-no-vote.active {
  background: #77c7f7;
}

.vue-poll .ans-cnt .ans-no-vote.active .txt {
  color: #fff;
}

.vue-poll .ans-cnt .ans-voted {
  padding: 5px 0;
}

.vue-poll .ans-cnt .ans-voted .percent,
.vue-poll .ans-cnt .ans-voted .txt {
  position: relative;
  z-index: 1;
}
.vue-poll .ans-cnt .ans-voted .percent {
  font-weight: bold;
  min-width: 51px;
  display: inline-block;
  margin: 0 10px;
}

.vue-poll .ans-cnt .ans-voted.selected .txt:after {
  content: "✔";
  margin-left: 10px;
}

.vue-poll .ans-cnt .ans .bg {
  position: absolute;
  width: 0%;
  top: 0;
  left: 0;
  bottom: 0;
  z-index: 0;
  background-color: #e1e8ed;
  border-top-left-radius: 5px;
  border-bottom-left-radius: 5px;
  transition: all 0.3s cubic-bezier(0.5, 1.2, 0.5, 1.2);
  -webkit-transition: all 0.3s cubic-bezier(0.5, 1.2, 0.5, 1.2);
  -moz-transition: all 0.3s cubic-bezier(0.5, 1.2, 0.5, 1.2);
}

.vue-poll .ans-cnt .ans .bg.selected {
  background-color: #77c7f7;
}

.vue-poll .votes {
  font-size: 14px;
  color: #8899a6;
}

.vue-poll .submit {
  display: block;
  text-align: center;
  margin: 0 auto;
  max-width: 80px;
  text-decoration: none;
  background-color: #41b882;
  color: #fff;
  padding: 10px 25px;
  border-radius: 5px;
}
</style>
