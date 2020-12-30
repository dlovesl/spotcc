<template>
        <play-count v-if="dataLoaded" v-bind="options"/>
</template>

<script>
  import PlayCount from "./PlayCount.vue";
  import Api from "../services/api";

  export default {
    components: {
       PlayCount 
    },
    data: function () {
          return {
            options: {
              accounts: [],
            },
            dataLoaded: false,
          };
        },
    methods: {
    fetchData() {
      let options = {};
      options.accounts = [];
      return new Promise((resolve) => {
        Api()
          .get(`/account/streamtype/0`)
          .then((res) => {
            //console.log(res);
            let result = res.data.map((r) =>{
              let acc = {};
              acc.artists = [];
              acc.name = r.name,
              acc.totalStreams = r.orderPreStreams
              acc.artists = r.artists.map((r) => r.spotifyId)
              return acc;
            });
            resolve(result);
          })
          .catch((error) => console.log(error));
      });
    },
  },
  mounted() {
    this.fetchData().then((data) => {
      //console.log(data);
      this.options.accounts = data;
      this.dataLoaded = true;
    });
  },
  }
</script>